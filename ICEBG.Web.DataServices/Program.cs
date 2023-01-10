using ICEBG.AppConfig;
using ICEBG.Client.Infrastructure.ClientServices;
using ICEBG.Web.DataServices;

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

#if BLAZOR_WEBASSEMBLY
    const string corsPolicy = "_corsPolicy";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: corsPolicy,
                          policy =>
                          {
                              policy.WithOrigins("https://localhost:7175",
                                                 "https://T570:7175",
                                                 "http://localhost:7175",
                                                 "http://T570:7175")
                                    .AllowCredentials()
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
                          });
    });
    builder.Services.AddCors();
#endif

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

    app.UseRouting();

#if BLAZOR_WEBASSEMBLY
    // This must be between UseRouting & UseEndpoints
    logger.Debug("UseCors...");
    app.UseCors(corsPolicy);
#endif

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
    NLog.LogManager.Shutdown();
}
