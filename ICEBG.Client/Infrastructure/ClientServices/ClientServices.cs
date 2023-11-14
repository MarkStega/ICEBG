//#if gRPC
using System.Net.Http;
using System.Security.Cryptography;

using Grpc.Net.Client;
using Grpc.Net.Client.Web;

using ICEBG.DataTier.gRPCClient;
using ICEBG.DataTier.Interfaces;

using Material.Blazor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
//#endif

namespace ICEBG.Client.Infrastructure.ClientServices;

public static class ClientServices
{
    private static ILogger<string> pLogger { get; set; } = null;

    public static void Inject(string baseUri, IServiceCollection serviceCollection)
    {
        //
        // Third party library services
        //
        pLogger?.LogInformation("Adding MBServices...");
        //serviceCollection.AddMBServices(options =>
        //{
        //    options.LoggingServiceConfiguration = new MBLoggingServiceConfiguration()
        //    {
        //        LoggingLevel = MBLoggingLevel.Debug
        //    };
        //    options.ToastServiceConfiguration = new MBToastServiceConfiguration()
        //    {
        //        InfoDefaultHeading = "Info",
        //        SuccessDefaultHeading = "Success",
        //        WarningDefaultHeading = "Warning",
        //        ErrorDefaultHeading = "Error",
        //        Timeout = 3000,
        //        MaxToastsShowing = 3,
        //        CloseMethod = MBNotifierCloseMethod.TimeoutAndDismissButton,
        //        Position = MBToastPosition.CenterLeft
        //    };
        //});

        // Work-around for missing service
        serviceCollection.AddMBServices();


        //
        // Data access services
        //

        // Create a gRPC-Web channel pointing to the backend server
        var httpClientHandler = new HttpClientHandler();
#if BLAZOR_SERVER
        httpClientHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        var channel = GrpcChannel.ForAddress(
            baseUri,
            new GrpcChannelOptions
            {
                HttpClient = new HttpClient(
                    new GrpcWebHandler(GrpcWebMode.GrpcWeb, httpClientHandler)),
                MaxReceiveMessageSize = null
            });
#else
        var channel = GrpcChannel.ForAddress(
            baseUri,
            new GrpcChannelOptions
            {
                HttpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, httpClientHandler),
                MaxReceiveMessageSize = null
            });
#endif

        pLogger?.LogInformation("Adding gRPC iConfigurationClient...");
        serviceCollection.AddScoped<iConfigurationClient, ConfigurationClientGRPC>();
        pLogger?.LogInformation("Adding gRPC ConfigurationProtoClient...");
        serviceCollection.AddScoped(user =>
        {
            return new ConfigurationProto.ConfigurationProtoClient(channel);
        });

        pLogger?.LogDebug("Add WeatherForecastService");
        serviceCollection.AddSingleton<WeatherForecastService>();

    }
}

