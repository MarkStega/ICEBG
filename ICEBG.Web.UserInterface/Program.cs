using System;
using System.IO.Compression;
using System.Threading.RateLimiting;

using Blazored.LocalStorage;

using CompressedStaticFiles.AspNet;

using GoogleAnalytics.Blazor;

using HttpSecurity.AspNet;

using ICEBG.AppConfig;
using ICEBG.Client.Infrastructure.ClientServices;
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
    ApplicationConfigurationServer.Initialize(builder);

    // Add services to the container.
    logger.Debug("ClientServices.Inject");
    ClientServices.Inject(ApplicationConfiguration.pGrpcEndpointPrefix, builder.Services);

    //  Response compression
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
    });

    // Performance test (performed in debug mode locally):
    // NoCompression - material.blazor.min.css takes circa 10 to 20 ms to download, 270 Kb - page load 95 to 210 ms - 3.2 MB transfered
    // Fastest - material.blazor.min.css takes circa 12 to 28 ms to download, 34.7 Kb - page load 250 to 270 ms - 2.2 MB transfered
    // SmallestSize & Optimal - material.blazor.min.css takes circa 500 to 800 ms to download, 16.2 Kb - page load 900 to 1100 ms (unacceptably slow) - 2.1 MB transfered
    builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Fastest;
    });

    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.SmallestSize;
    });

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
                        .AddUri((baseUri, baseDomain) => $"wss://{baseDomain}:*"))
                    // The generated hashes do nothing here, and we include it here only to show that generated hash values can be added to policies - script-src would generally be the policy where you use this technique.
                    .AddDefaultSrc(o => o
                        .AddSelf()
                        .AddStrictDynamicIf(() => !builder.Environment.IsDevelopment())
                        .AddUnsafeInline()
                        .AddGeneratedHashValues(StaticFileExtension.CSS))
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
                    .AddPrefetchSrc(o => o.AddSelf())
                    .AddObjectSrc(o => o.AddNone())
                    .AddReportUri(o => o.AddUri((baseUri, baseDomain) => $"https://{baseUri}/api/CspReporting/UriReport"))
                    // The sha-256 hash relates to an inline script added by blazor's javascript
                    .AddScriptSrc(o => o
                        .AddHashValue(HashAlgorithm.SHA256, "v8v3RKRPmN4odZ1CWM5gw80QKPCCWMcpNeOmimNL2AA=")
                        .AddUriIf((baseUri, baseDomain) => $"https://{baseUri}/_framework/aspnetcore-browser-refresh.js", () => builder.Environment.IsDevelopment())
                        .AddSelfIf(() => builder.Environment.IsDevelopment() || PlatformDetermination.kIsBlazorWebAssembly)
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
            .AddAccessControlAllowOriginAll()
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

    builder.Services.AddCompressedStaticFiles();

    logger.Debug("Add Grpc");
    builder.Services.AddGrpc(options =>
    {
        options.EnableDetailedErrors = true;
        options.MaxReceiveMessageSize = null;
        options.MaxSendMessageSize = null;
    });

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

    app.UseResponseCompression();

    app.UseCookiePolicy();

    app.UseHttpsRedirection();

    //    app.UseHttpSecurityHeaders();

#if BLAZOR_SERVER
    app.MapBlazorHub();
#else
    app.UseBlazorFrameworkFiles();
#endif

    app.UseCompressedStaticFiles();

    app.UseRouting();

    // Limit api calls to 10 in a second to prevent external denial of service.
    app.UseRateLimiter(new()
    {
        GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            return RateLimitPartition.GetFixedWindowLimiter("GeneralLimit",
                _ => new FixedWindowRateLimiterOptions()
                {
                    Window = TimeSpan.FromSeconds(1),
                    PermitLimit = 1,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 10,
                });
        }),
        RejectionStatusCode = 429,
    });

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
    //NLog.LogManager.Shutdown();
}
