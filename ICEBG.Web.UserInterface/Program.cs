using System;
using System.IO.Compression;
using System.Threading.RateLimiting;

using Blazored.LocalStorage;

using CompressedStaticFiles.AspNet;

using GoogleAnalytics.Blazor;

using ICEBG.AppConfig;
using ICEBG.Client;
using ICEBG.Client.Infrastructure.ClientServices;
using ICEBG.Web.UserInterface;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NLog;
using NLog.Web;

// NLog: setup the logger first to catch all errors
Logger logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

try
{
    logger.Debug("______________________________________________________________________");
    logger.Debug("Building and Starting Host in Main() for ICEBG.Web.UserInterface");
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    logger.Debug("");

    logger.Debug("ApplicationConfiguration.Initialize");
    ApplicationConfigurationServer.Initialize(builder);

    // Add services to the container.
    logger.Debug("ClientServices.Inject");
    ClientServices.Inject(ApplicationConfiguration.pDataServicesEndpointPrefix, builder.Services);

    builder.Services.AddResponseCaching();

    logger.Debug("Adding razor pages");
    builder.Services.AddRazorPages();

    // Needed for prerendering on WebAssembly as well as general use
    builder.Services.AddTransient<INotification, ServerNotificationService>();

#if BLAZOR_SERVER
    logger.Debug("AddMvc");
    builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

    logger.Debug("Add server side blazor");
    builder.Services.AddServerSideBlazor();
#endif

    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
    });

    logger.Debug("Configure<CookiePolicyOptions>");
    builder.Services.Configure<CookiePolicyOptions>(options =>
    {
        options.CheckConsentNeeded = context => true;
        options.HttpOnly = HttpOnlyPolicy.Always;
        options.MinimumSameSitePolicy = SameSiteMode.Strict;
        options.Secure = CookieSecurePolicy.Always;
    });

    logger.Debug("AddOptions");
    builder.Services.AddOptions();

    logger.Debug("AddMemoryCache");
    builder.Services.AddMemoryCache();

    logger.Debug("AddHttpContextAccessor");
    builder.Services.AddHttpContextAccessor();

    logger.Debug("AddBlazoredLocalStorage");
    builder.Services.AddBlazoredLocalStorage();

    logger.Debug("AddGBService");
    builder.Services.AddGBService(options =>
    {
        options.TrackingId = "G-2VZJ2X14RH";
    });

    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.AddServerHeader = false;
    });

    logger.Debug("Add Grpc");
    builder.Services.AddGrpc(options =>
    {
        options.EnableDetailedErrors = true;
        options.MaxReceiveMessageSize = null;
        options.MaxSendMessageSize = null;
    });

    // Add compressed static files service 
    builder.Services.AddCompressedStaticFiles();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
#if BLAZOR_SERVER
        app.UseDeveloperExceptionPage();
#else
        app.UseWebAssemblyDebugging();
#endif
    }
    else
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseCompressedStaticFiles();

    app.UseHttpsRedirection();

#if BLAZOR_SERVER
    app.MapBlazorHub();
#else
    app.UseBlazorFrameworkFiles();
#endif

    app.UseRouting();

    app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

    app.MapControllers();

    app.MapFallbackToPage("/host");

    logger.Debug("Completing startup, executing app.Run()...");
    logger.Debug(" ");
    await app.RunAsync();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Unhandled exception");
}
finally
{
    // Ensure message flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    logger.Debug("Shutting down NLOG");
    LogManager.Shutdown();
}
