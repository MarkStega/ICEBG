using System;
using System.IO.Compression;
using System.Threading.RateLimiting;

using Blazored.LocalStorage;

using CompressedStaticFiles.AspNet;

using GoogleAnalytics.Blazor;

using HttpSecurity.AspNet;

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

    logger.Debug("AddHttpContextAccessor");
    builder.Services.AddHttpContextAccessor();

    logger.Debug("AddBlazoredLocalStorage");
    builder.Services.AddBlazoredLocalStorage();

    logger.Debug("AddGBService");
    builder.Services.AddGBService(options =>
    {
        options.TrackingId = "G-2VZJ2X14RH";
    });

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

    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
    });

    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.AddServerHeader = false;
    });

    builder.Services.AddCompressedStaticFiles();

    // needed to store rate limit counters and ip rules
    builder.Services.AddMemoryCache();

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
                    .AddFontSrc(o => o.AddSelf())
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
                    .AddScriptSrc(o => o
                        .AddSelf()
                        .AddNonce()
                        .AddHashValue(HashAlgorithm.SHA256, "v8v3RKRPmN4odZ1CWM5gw80QKPCCWMcpNeOmimNL2AA=")
                        .AddUriIf((baseUri, baseDomain) => $"https://{baseUri}/_framework/aspnetcore-browser-refresh.js", () => builder.Environment.IsDevelopment())
                        .AddStrictDynamicIf(() => !builder.Environment.IsDevelopment())
                        .AddUnsafeInline().AddReportSample().AddUnsafeEval().AddUri("https://www.googletagmanager.com/gtag/js")
                        .AddGeneratedHashValues(StaticFileExtension.JS))
                    .AddStyleSrc(o => o
                        .AddSelf()
                        .AddUnsafeInline()
                        .AddUnsafeHashes()
                        .AddReportSample())
                    .AddUpgradeInsecureRequests()
                    .AddWorkerSrc(o => o.AddSelf());
            })
            .AddAccessControlAllowOriginSingle("a.com")
            .AddReferrerPolicy(ReferrerPolicyDirective.NoReferrer)
            .AddPermissionsPolicy("accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()")
            .AddStrictTransportSecurity(31536000, true)
            .AddXClientId("ICEBG.Web.DataServices")
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
        app.UseHsts();
    }

    app.UseResponseCompression();

    app.UseCookiePolicy();

    app.UseHttpsRedirection();

    app.UseCompressedStaticFiles();

    app.UseContentSecurityPolicy();

    app.UseMiddleware<NoCacheMiddleware>();

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

    app.UseAuthentication();

    app.UseAuthorization();

#if BLAZOR_SERVER
    app.MapBlazorHub();
#endif

    app.MapFallbackToPage("/host");

    app.MapGet("/sitemap.xml", async context => { await Sitemap.Generate(context); });

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
    //NLog.LogManager.Shutdown();
}
