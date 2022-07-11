using System;
using System.IO.Compression;

using AspNetCoreRateLimit;

using Blazored.LocalStorage;

using GoogleAnalytics.Blazor;

using ICEBG.Client;
using ICEBG.Infrastructure.ClientServices;
using ICEBG.SystemFramework;
using ICEBG.Web.UserInterface;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NLog.Web;


// NLog: setup the logger first to catch all errors
NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

try
{
    logger.Debug("______________________________________________________________________");
    logger.Debug("Building and Starting Host in Main()");
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(LogLevel.Trace);
    builder.Host.UseNLog();

    logger.Debug("");

    logger.Debug("ApplicationConfiguration.Initialize");
    ApplicationConfiguration.Initialize(builder);

    // Add services to the container.
    logger.Debug("ClientServices.Inject");
    ClientServices.Inject(ApplicationConfiguration.pGrpcEndpointPrefix, builder.Services);

    logger.Debug("ResponseCompression");
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
    });

    builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Fastest;
    });

    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.SmallestSize;
    });

    logger.Debug("Adding razor pages");
    builder.Services.AddRazorPages();

    logger.Debug("Add server side blazor");
    builder.Services.AddServerSideBlazor();

    logger.Debug("Add WeatherForecastService");
    builder.Services.AddSingleton<WeatherForecastService>();

    builder.Services.AddScoped<NonceService>();

    logger.Debug("Add Grpc");
    builder.Services.AddGrpc(options =>
    {
        options.EnableDetailedErrors = true;
        options.MaxReceiveMessageSize = null;
        options.MaxSendMessageSize = null;
    });

    builder.Services.Configure<CookiePolicyOptions>(options =>
    {
        options.CheckConsentNeeded = context => true;
        options.HttpOnly = HttpOnlyPolicy.Always;
        options.MinimumSameSitePolicy = SameSiteMode.Strict;
        options.Secure = CookieSecurePolicy.Always;
    });

    builder.Services.AddOptions();
    // needed to store rate limit counters and ip rules
    builder.Services.AddMemoryCache();

    //load general configuration from appsettings.json
    builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));

    //load client rules from appsettings.json
    builder.Services.Configure<ClientRateLimitPolicies>(builder.Configuration.GetSection("ClientRateLimitPolicies"));

    builder.Services.AddInMemoryRateLimiting();

    // configuration (resolvers, counter key builders)
    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddBlazoredLocalStorage();

    builder.Services.AddGBService(trackingId: "G-2VZJ2X14RH");

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        builder.Services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseBlazorFrameworkFiles();

    app.Use(async (context, next) =>
    {
        var nonceValue = context.RequestServices.GetService<NonceService>()?.NonceValue ?? throw new Exception("Nonce service unavailable");
        logger.Debug("Middleware csp invoked for '" + context.Request.Path + "'");
        logger.Debug("                           '" + nonceValue + "'");

        var baseUri = context.Request.Host.ToString();
        var baseDomain = context.Request.Host.Host;

        var csp =
            "base-uri 'self'; " +
            "block-all-mixed-content; " +
            "child-src 'self' ; " +
            $"connect-src 'self' wss://{baseDomain}:* www.google-analytics.com; " +
            "default-src 'self'; " +
            "font-src fonts.gstatic.com; " +
            "form-action 'none'; " +
            "frame-ancestors 'none'; " +
            "frame-src 'self'; " +
            "img-src 'self' www.google-analytics.com; " +
            "manifest-src 'self'; " +
            "media-src 'self'; " +
            "prefetch-src 'self'; " +
            "object-src 'none'; " +
            $"report-to https://{baseUri}/api/CspReporting/UriReport; " +
            $"report-uri https://{baseUri}/api/CspReporting/UriReport; " +
            $"script-src 'self' 'report-sample' ;" +
            "style-src 'self' 'report-sample'; " +
            "upgrade-insecure-requests; " +
            "worker-src 'self';";

        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
        context.Response.Headers.Add("X-ClientId", "dioptra");
        context.Response.Headers.Add("Referrer-Policy", "no-referrer");
        context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
        context.Response.Headers.Add("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
        context.Response.Headers.Add("Content-Security-Policy", csp);

        await next();
    });

    app.UseStaticFiles();

    app.UseMiddleware<NoCacheMiddleware>();

    app.UseRouting();

    app.UseClientRateLimiting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapBlazorHub();

    app.MapFallbackToPage("/index_server");

    app.MapGet("/sitemap.xml", async context => {
        await Sitemap.Generate(context);
    });

    logger.Debug("Completing startup, executing app.Run()...");
    logger.Debug(" ");
    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Unhandled exception");
}
finally
{
    // Ensure message flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    logger.Debug("Shutting down NLOG");
    NLog.LogManager.Shutdown();
}
