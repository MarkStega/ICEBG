#define OptimiserInDevelopment
#define PSSProxy

using Microsoft.AspNetCore.Builder;

//
//  2018-06-29  Mark Stega
//              Created
//
//  2022-08-13  Mark Stega
//              Added PSSProxy configuration for LBH
//

namespace ICEBG.AppConfig;
public static class ApplicationConfigurationServer
{
    #region Methods

    public static void Initialize(WebApplicationBuilder builder)
    {
        ApplicationConfiguration.Initialize();

        ApplicationConfiguration.pConfigurationIdentifier = builder.Configuration["ICEBG:BaseConfiguration:ConfigurationIdentifier"];
        ApplicationConfiguration.pDataServicesEndpointPrefix = builder.Configuration["ICEBG:BaseConfiguration:DataServicesEndpointPrefix"];
        ApplicationConfiguration.pSqlConnectionString = builder.Configuration["ICEBG:BaseConfiguration:SqlConnectionString"];
        ApplicationConfiguration.pWeatherEndpoint = builder.Configuration["ICEBG:BaseConfiguration:WeatherEndpoint"];
    }

    #endregion
}

