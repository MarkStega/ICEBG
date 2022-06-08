using System;

using Grpc.Core;

using ICEBG.SystemFramework;
using ICEBG.Web.DataServices.Services.Configuration;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    ApplicationConfiguration.Initialize(builder);

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console()
        .WriteTo.File("c:/temp/Serilog.Data.log"));
    
    // Add services to the container.

    builder.Services.AddRazorPages();

    Log.Debug("Adding gRPC...");
    builder.Services.AddGrpc(options =>
    {
        options.EnableDetailedErrors = true;
        options.MaxReceiveMessageSize = null;
        options.MaxSendMessageSize = null;
    });
    builder.Services.AddGrpcReflection();

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

    app.UseRouting();

    // This must be between UseRouting & UseEndpoints
    Log.Debug("UseGrpcWeb...");
    app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGrpcService<ConfigurationService>();
        endpoints.MapFallbackToPage("/index_server");
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
