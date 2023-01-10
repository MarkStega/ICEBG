using Material.Blazor;

//
//  2018-06-29  Mark Stega
//              Created
//

namespace ICEBG.AppConfig;

public static class ApplicationConfigurationWASM
{
    #region Methods

    public static void Initialize()
    {
        ApplicationConfiguration.Initialize();

        #region AZURE
#if AZURE
        ApplicationConfiguration.pGrpcEndpointPrefix = "https://waicebg-ds.azurewebsites.net/";
        ApplicationConfiguration.pConfigurationIdentifier = "ICEBG";
        ApplicationConfiguration.pSqlConnectionString = "n/a";
        ApplicationConfiguration.pWeatherEndpoint = "https://waicebg-ds.azurewebsites.net/WeatherForecast";
#endif
        #endregion

        #region DEVELOP
#if DEVELOP
        ApplicationConfiguration.pGrpcEndpointPrefix = "https://localhost:7173/";
        ApplicationConfiguration.pConfigurationIdentifier = "ICEBG";
        ApplicationConfiguration.pSqlConnectionString = "n/a";
        ApplicationConfiguration.pWeatherEndpoint = "https://localhost:7173/WeatherForecast";
#endif
        #endregion

        #region WASM
#if WASM

        // GRPC
        ApplicationConfiguration.pGRPC_EndpointPrefix = "https://localhost:44350/";

        // MB
        ApplicationConfiguration.pMB_LoggingLevel = MBLoggingLevel.Warning;

        // UI
        ApplicationConfiguration.pUI_AutoLogoffInterval = 1440;
        ApplicationConfiguration.pUI_PMIProtection = false;
#endif
        #endregion

    }

    #endregion
}

