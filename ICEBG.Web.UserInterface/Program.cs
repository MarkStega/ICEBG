using System;
using System.IO.Compression;
using System.Threading.RateLimiting;

using Blazored.LocalStorage;

using CompressedStaticFiles.AspNet;

using GoogleAnalytics.Blazor;

using HttpSecurity.AspNet;

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

    builder.Services.AddHttpsSecurityHeaders(options =>
    {
        options
            .AddContentSecurityOptions(cspOptions =>
            {
                cspOptions
                    .AddBaseUri(o => o.AddSelf())
                    .AddBlockAllMixedContent()
                    .AddChildSrc(o => o.AddSelf())

                    .AddConnectSrc(o => o
                        .AddSelf()
                        .AddUri((baseUri, baseDomain) => $"wss://{baseDomain}:*")
                        .AddUri((baseUri, baseDomain) => ApplicationConfiguration.pDataServicesEndpointPrefix))

                    // The generated hashes do nothing here, and we include it here only to show that generated hash values can be added to policies - script-src would generally be the policy where you use this technique.
                    .AddDefaultSrc(o => o
                        .AddSelf()
                        .AddStrictDynamicIf(() => !builder.Environment.IsDevelopment())
                        .AddUnsafeInline()
                        .AddGeneratedHashValues(StaticFileExtension.CSS)
                        .AddUri((baseUri, baseDomain) => ApplicationConfiguration.pDataServicesEndpointPrefix))

                    .AddFontSrc(o => o
                        .AddUri("https://fonts.googleapis.com")
                        .AddUri("https://fonts.gstatic.com"))
                    .AddFrameAncestors(o => o.AddNone())
                    .AddFrameSrc(o => o.AddSelf())
                    .AddFormAction(o => o.AddNone())
                    .AddImgSrc(o => o
                        .AddSelf()
                        .AddUri("www.google-analytics.com")
                        .AddSchemeSource(SchemeSource.Data, "w3.org/svg/2000"))
                    .AddManifestSrc(o => o.AddSelf())
                    .AddMediaSrc(o => o.AddSelf())
                    .AddObjectSrc(o => o.AddNone())
                    .AddReportUri(o => o.AddUri((baseUri, baseDomain) => $"https://{baseUri}/api/CspReporting/UriReport"))
                    // The first sha-256 hash relates to an inline script added by blazor's javascript
                    // The second sha-256 hash relates to material.blazor.md3.lib.module.js
                    .AddScriptSrc(o => o
                        .AddHashValue(HashAlgorithm.SHA256, "v8v3RKRPmN4odZ1CWM5gw80QKPCCWMcpNeOmimNL2AA=")
                        .AddHashValue(HashAlgorithm.SHA256, "D3eUfxVDJsvQ4e7E3LQLh/d/B1BumEUYYuuYq3QCjW4=")
                        .AddUriIf((baseUri, baseDomain) => $"https://{baseUri}/_framework/aspnetcore-browser-refresh.js", () => builder.Environment.IsDevelopment())
                        //.AddSelfIf(() => builder.Environment.IsDevelopment() || PlatformDetermination.kIsBlazorWebAssembly)
                        //.AddSelf()
                        //.AddStrictDynamicIf(() => !builder.Environment.IsDevelopment() && PlatformDetermination.IsBlazorWebAssembly) // this works on Chromium browswers but fails for both Firefox and Safari
                        .AddUnsafeInlineIf(() => PlatformDetermination.kIsBlazorWebAssembly)
                        .AddReportSample()
                        .AddUnsafeEvalIf(() => PlatformDetermination.kIsBlazorWebAssembly)
                        .AddUri("https://www.googletagmanager.com/gtag/js")
                        .AddUri((baseUri, baseDomain) => $"https://{baseUri}/_content/GoogleAnalytics.Blazor/googleanalytics.blazor.js") // Required to work on Safari
                        .AddUri((baseUri, baseDomain) => $"https://{baseUri}/_content/Material.Blazor/material.blazor.min.js") // Required to work on Safari
                        .AddUriIf((baseUri, baseDomain) => $"https://{baseUri}/_framework/blazor.server.js", () => PlatformDetermination.kIsBlazorServer) // Required to work on Safari
                        .AddUriIf((baseUri, baseDomain) => $"https://{baseUri}/_framework/blazor.webassembly.js", () => PlatformDetermination.kIsBlazorWebAssembly) // Required to work on Safari
                        .AddGeneratedHashValues(StaticFileExtension.JS))
                    .AddStyleSrc(o => o
                        .AddSelf()
                        .AddUnsafeInline()
                        .AddUnsafeHashes()
                        .AddReportSample())
                    .AddUpgradeInsecureRequests()
                    .AddWorkerSrc(o => o.AddSelf());
            })
            .AddReferrerPolicy(ReferrerPolicyDirective.NoReferrer)
            .AddPermissionsPolicy("accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()")
            .AddStrictTransportSecurity(31536000, true)
            .AddXClientId("ICEBG.Web.UserInterface")
            .AddXContentTypeOptionsNoSniff()
            .AddXFrameOptionsDirective(XFrameOptionsDirective.Deny)
            .AddXXssProtectionDirective(XXssProtectionDirective.OneModeBlock)
            .AddXPermittedCrossDomainPoliciesDirective(XPermittedCrossDomainPoliciesDirective.None);
    },
    onStartingOptions =>
    {
        onStartingOptions
            .AddCacheControl("max-age=86400, no-cache, public")
            .AddExpires("0");
    });

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

    builder.Services.AddRateLimiter(_ => _
        .AddFixedWindowLimiter(policyName: "fixed", options =>
        {
            options.PermitLimit = 1;
            options.Window = TimeSpan.FromSeconds(1);
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            options.QueueLimit = 10;
        }
        ));

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

    app.UseCookiePolicy();

    app.UseHttpsRedirection();

    app.UseHttpSecurityHeaders();

#if BLAZOR_SERVER
    app.MapBlazorHub();
#else
    app.UseBlazorFrameworkFiles();
#endif

    app.UseRouting();

    // Limit api calls to 10 in a second to prevent external denial of service.
    app.UseRateLimiter();

    app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

    app.MapControllers();

    app.MapFallbackToPage("/host");

    app.MapGet("/sitemap.xml", async context => { await Sitemap.Generate(context); });

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
