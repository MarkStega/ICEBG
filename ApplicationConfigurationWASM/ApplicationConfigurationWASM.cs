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
        // GRPC
        ApplicationConfiguration.pGRPC_EndpointPrefix = "https://localhost:443/";

        // MB
        ApplicationConfiguration.pMB_LoggingLevel = MBLoggingLevel.Warning;

        // UI
        ApplicationConfiguration.pUI_AutoLogoffInterval = 1440;
        ApplicationConfiguration.pUI_PMIProtection = true;
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

        #region LBH
#if LBH
        // GRPC
        ApplicationConfiguration.pGRPC_EndpointPrefix = "https://localhost:44350/";

        // MB
        ApplicationConfiguration.pMB_LoggingLevel = MBLoggingLevel.Warning;

        // UI
        ApplicationConfiguration.pUI_AutoLogoffInterval = 60;
        ApplicationConfiguration.pUI_PMIProtection = false;
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

