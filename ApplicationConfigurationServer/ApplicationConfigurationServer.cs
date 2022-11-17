#define OptimiserInDevelopment
#define PSSProxy

using System;

using Material.Blazor;

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
        ApplicationConfiguration.pGrpcEndpointPrefix = builder.Configuration["ICEBG:BaseConfiguration:GrpcEndpointPrefix"];
        ApplicationConfiguration.pSqlConnectionString = builder.Configuration["ICEBG:BaseConfiguration:SqlConnectionString"];
        ApplicationConfiguration.pWeatherEndpoint = builder.Configuration["ICEBG:BaseConfiguration:WeatherEndpoint"];
    }

    #endregion
}

