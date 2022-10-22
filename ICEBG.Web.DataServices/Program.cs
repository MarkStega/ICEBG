using System;
using System.IO.Compression;
using System.Threading.RateLimiting;

using CompressedStaticFiles;

using HttpSecurity.AspNet;

using ICEBG.Infrastructure.ClientServices;
using ICEBG.SystemFramework;
using ICEBG.Web.DataServices;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

    builder.Services.AddHttpsSecurityHeaders(
        options => OptionsBuilder.BuildGeneralHeaderOptions(builder, options),
                   onStartupOptions =>
                                      OptionsBuilder.BuildOnStartupHeaderOptions(builder,
                                      onStartupOptions));

    builder.Services.AddCompressedStaticFiles();

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

    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseHttpSecurityHeaders();

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
