#define OptimiserInDevelopment
#define PSSProxy

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

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

    public static void Initialize(IConfiguration configuration = null)
    {
        ApplicationConfiguration.Initialize();

        ApplicationConfiguration.pConfigurationIdentifier = configuration?.GetSection("ICEBG:BaseConfiguration:ConfigurationIdentifier").Value ?? string.Empty;
        ApplicationConfiguration.pDataServicesEndpointPrefix = configuration?.GetSection("ICEBG:BaseConfiguration:DataServicesEndpointPrefix").Value ?? string.Empty;
        ApplicationConfiguration.pSqlConnectionString = configuration?.GetSection("ICEBG:BaseConfiguration:SqlConnectionString").Value ?? string.Empty;
        ApplicationConfiguration.pWeatherEndpoint = configuration?.GetSection("ICEBG:BaseConfiguration:WeatherEndpoint").Value ?? string.Empty;
    }

    #endregion
}

