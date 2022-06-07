using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

//
//  2018-06-29  Mark Stega
//              Created
//

namespace ICEBG.SystemFramework;

public static class ApplicationConfiguration
{
    #region Properties

    public static string pConfigurationIdentifier { get; set; }
    public static string pGrpcEndpointPrefix { get; set; }
    public static string pWeatherEndpoint { get; set; }

    #endregion

    #region Methods
    public static void Initialize(WebApplicationBuilder builder)
    {
        pConfigurationIdentifier = builder.Configuration["ICEBG:BaseConfiguration:ConfigurationIdentifier"];
        pGrpcEndpointPrefix = builder.Configuration["ICEBG:BaseConfiguration:GrpcEndpointPrefix"];
        pWeatherEndpoint = builder.Configuration["ICEBG:BaseConfiguration:WeatherEndpoint"];
    }

    #endregion
}

