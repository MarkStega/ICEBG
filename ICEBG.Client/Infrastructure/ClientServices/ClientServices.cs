using Material.Blazor;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

//#if gRPC
using System.Net.Http;

using Grpc.Net.Client;
using Grpc.Net.Client.Web;

using ICEBG.DataTier.gRPCClient;
using ICEBG.DataTier.Interfaces;
//#endif

namespace ICEBG.Infrastructure.ClientServices;

public static class ClientServices
{
    private static ILogger<string> pLogger { get; set; } = null;

    public static void Inject(string baseUri, IServiceCollection serviceCollection)
    {
        //
        // Third party library services
        //
        pLogger?.LogInformation("Adding MBServices...");
        serviceCollection.AddMBServices(
            loggingServiceConfiguration : new MBLoggingServiceConfiguration()
            {
                LoggingLevel = MBLoggingLevel.Debug
            },
            toastServiceConfiguration: new MBToastServiceConfiguration()
            {
                InfoDefaultHeading = "Info",
                SuccessDefaultHeading = "Success",
                WarningDefaultHeading = "Warning",
                ErrorDefaultHeading = "Error",
                Timeout = 3000,
                MaxToastsShowing = 3,
                CloseMethod = MBNotifierCloseMethod.TimeoutAndDismissButton,
                Position = MBToastPosition.CenterLeft
                }
            );

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
    }
}

