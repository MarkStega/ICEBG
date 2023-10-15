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
    public static string pDataServicesEndpointPrefix { get; set; }
    public static string pSqlConnectionString { get; set; }
    public static string pWeatherEndpoint { get; set; }

    #endregion

    #region Initialize

    public static void Initialize()
    {
        pAPP_ReleaseDate = "01/16/2023";
        pAPP_Version = "3.2.10";
    }

    #endregion
}

