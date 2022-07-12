using System;
using System.IO.Compression;

using AspNetCoreRateLimit;

using Blazored.LocalStorage;

using GoogleAnalytics.Blazor;

using ICEBG.Client;
using ICEBG.Infrastructure.ClientServices;
using ICEBG.SystemFramework;
using ICEBG.Web.UserInterface;
using ICEBG.Web.UserInterface.Middleware;
using ICEBG.Web.UserInterface.Services;

using Material.Blazor;

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

    logger.Debug("AddControllersWithViews");
    builder.Services.AddControllersWithViews();

    logger.Debug("AddMvc");
    builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

    logger.Debug("AddMBServices");
    builder.Services.AddMBServices(loggingServiceConfiguration: Utilities.GetDefaultLoggingServiceConfiguration(), toastServiceConfiguration: Utilities.GetDefaultToastServiceConfiguration(), snackbarServiceConfiguration: Utilities.GetDefaultSnackbarServiceConfiguration());

    logger.Debug("AddHttpClient");
    builder.Services.AddHttpClient();

    logger.Debug("ContentSecurityPolicyService");
    builder.Services.AddSingleton<ContentSecurityPolicyService>();

    logger.Debug("Configure<CookiePolicyOptions>");
    builder.Services.Configure<CookiePolicyOptions>(options =>
    {
        options.CheckConsentNeeded = context => true;
        options.HttpOnly = HttpOnlyPolicy.Always;
        options.MinimumSameSitePolicy = SameSiteMode.Strict;
        options.Secure = CookieSecurePolicy.Always;
    });

    logger.Debug("Configure<StaticFileOptions>");
    builder.Services.Configure<StaticFileOptions>(options =>
    {
        options.OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Add("Cache-Control", "public, max-age=86400");
            ctx.Context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        };
    });

    logger.Debug("AddOptions");
    builder.Services.AddOptions();

    logger.Debug("AddMemoryCache");
    builder.Services.AddMemoryCache();

    logger.Debug("Configure<ClientRateLimitOptions>");
    builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));

    logger.Debug("Configure<ClientRateLimitPolicies>");
    builder.Services.Configure<ClientRateLimitPolicies>(builder.Configuration.GetSection("ClientRateLimitPolicies"));

    logger.Debug("AddInMemoryRateLimiting");
    builder.Services.AddInMemoryRateLimiting();

    logger.Debug("AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>");
    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

    logger.Debug("AddHttpContextAccessor");
    builder.Services.AddHttpContextAccessor();

    logger.Debug("AddBlazoredLocalStorage");
    builder.Services.AddBlazoredLocalStorage();

    logger.Debug("AddGBService");
    builder.Services.AddGBService(trackingId: "G-2VZJ2X14RH");

    logger.Debug("Add server side blazor");
    builder.Services.AddServerSideBlazor();

    logger.Debug("Add WeatherForecastService");
    builder.Services.AddSingleton<WeatherForecastService>();

    logger.Debug("Add Grpc");
    builder.Services.AddGrpc(options =>
    {
        options.EnableDetailedErrors = true;
        options.MaxReceiveMessageSize = null;
        options.MaxSendMessageSize = null;
    });

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

    app.UseResponseCompression();

    app.UseCookiePolicy();

    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseContentSecurityPolicy();

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
