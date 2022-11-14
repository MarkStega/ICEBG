using System;

using Material.Blazor;

//
//  2018-06-29  Mark Stega
//              Created
//

namespace ICEBG.AppConfig;

public static class ApplicationConfiguration
{
    #region Properties

    public static string pAPP_ReleaseDate { get; set; }
    public static string pAPP_Version { get; set; }


    public static string pConfigurationIdentifier { get; set; }
    public static string pGrpcEndpointPrefix { get; set; }
    public static string pSqlConnectionString { get; set; }
    public static string pWeatherEndpoint { get; set; }

    #endregion

    #region Initialize

    public static void Initialize()
    {
        pAPP_ReleaseDate = "10/09/2022";
        pAPP_Version = "3.2.9";
    }

    #endregion
}

