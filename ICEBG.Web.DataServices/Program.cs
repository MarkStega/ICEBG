using System.IO.Compression;
using System.Threading.RateLimiting;

using HttpSecurity.AspNet;

using ICEBG.Infrastructure.ClientServices;
using ICEBG.SystemFramework;
using ICEBG.Web.DataServices;

using Microsoft.AspNetCore.ResponseCompression;

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

    logger.Debug("ClientServices.Inject");
    ClientServices.Inject(ApplicationConfiguration.pGrpcEndpointPrefix, builder.Services);

    // Add services to the container.

    logger.Debug("Adding razor pages");
    builder.Services.AddRazorPages();

    logger.Debug("Adding gRPC");
    builder.Services.AddGrpc(options =>
    {
        options.EnableDetailedErrors = true;
        options.MaxReceiveMessageSize = null;
        options.MaxSendMessageSize = null;
    });
    builder.Services.AddGrpcReflection();

    // needed to store rate limit counters and ip rules
    builder.Services.AddMemoryCache();

    //    builder.Services.AddHttpsSecurityHeaders(options => OptionsBuilder.BuildGeneralHeaderOptions(builder, options), onStartupOptions => OptionsBuilder.BuildOnStartupHeaderOptions(builder, onStartupOptions));

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
        //app.UseHsts();
    }

    //app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseHttpSecurityHeaders();

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

    // This must be between UseRouting & UseEndpoints
    logger.Debug("UseGrpcWeb");
    app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapGrpcService<ConfigurationService>();

    app.MapFallbackToPage("/host");

    logger.Debug("Completing startup, executing app.Run()");
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
