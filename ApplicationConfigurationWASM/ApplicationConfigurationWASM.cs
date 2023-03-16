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
        ApplicationConfiguration.pConfigurationIdentifier = "ICEBG";
        ApplicationConfiguration.pDataServicesEndpointPrefix = "https://waicebg-ds.azurewebsites.net/";
        ApplicationConfiguration.pSqlConnectionString = "n/a";
        ApplicationConfiguration.pWeatherEndpoint = "WeatherForecast";
#endif
        #endregion

        #region DEVELOP
#if DEVELOP
        ApplicationConfiguration.pConfigurationIdentifier = "ICEBG";
        ApplicationConfiguration.pDataServicesEndpointPrefix = "https://localhost:7173/";
        ApplicationConfiguration.pSqlConnectionString = "n/a";
        ApplicationConfiguration.pWeatherEndpoint = "WeatherForecast";
#endif
        #endregion
    }

    #endregion
}

