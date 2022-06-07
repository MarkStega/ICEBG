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

public static class ApplicationBaseConfiguration
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

public static class ApplicationConfigurationOld
{
    #region Properties
    public static string pApplicationReleaseDate { get; private set; }
    public static string pApplicationVersion { get; private set; }
    public static bool pPSS_ArchivistEnabled { get; private set; }
    public static int pPSS_ArchivistSpan { get; private set; }
    public static int pUI_AutoLogoffInterval { get; set; }
    public static string pWEB_DocumentsDirectory { get; private set; }
    public static string pWEB_DocumentsURLPrefix { get; private set; }
    public static string pGRPC_Endpoint { get; private set; }


    public static string pCS_Phaxio_Key { get; private set; }
    public static string pCS_Phaxio_Secret { get; private set; }
    public static bool pCS_SMTP_EnableSSL { get; private set; }
    public static string pCS_SMTP_ServerName { get; private set; }
    public static int pCS_SMTP_ServerPort { get; private set; }
    public static string pCS_SMTP_UserName { get; private set; }
    public static string pCS_SMTP_UserPassword { get; private set; }


    public static int pFCS_CleanupAge { get; private set; }
    public static string pFCS_CleanupTargets { get; private set; }


    public static bool pLFIS_Enable { get; private set; }
    public static string pLFIS_FTPDirectory_Clinic { get; private set; }
    public static string pLFIS_FTPDirectory_OPIS { get; private set; }
    public static bool pLFIS_Log_RawData { get; private set; }
    public static bool pLFIS_Log_MOID { get; private set; }


    public static bool pPFIS_Enable { get; private set; }
    public static string pPFIS_FTPDirectory { get; private set; }
    public static bool pPFIS_Log_RawData { get; private set; }
    public static bool pPFIS_Log_SPD { get; private set; }


    public static int pPSS_MaintenanceHour { get; private set; }
    public static bool pPSS_ProcessLinkerEnabled { get; private set; }
    public static bool pPSS_ProcessLinkerUpdateArchive { get; private set; }
    public static bool pPSS_ProcessLinkerUpdateProcesses { get; private set; }


    public static string pRS_ReportsDirectory { get; private set; }
    public static string pRS_ReportsURLPrefix { get; private set; }
    public static bool pRS_UpdateInfusionData { get; private set; }
    public static bool pRS_WABAK_Enabled { get; private set; }
    public static DateTime pRS_WABAK_Date { get; private set; }
    public static bool pRS_WABAK_UpdateDashboards { get; private set; }


    public static string pSMS_SystemIdentifier { get; private set; }
    public static bool pSMS_Disable0100Report { get; private set; }
    public static bool pSMS_Enable_LFIS { get; private set; }
    public static bool pSMS_Enable_PFIS { get; private set; }
    public static bool pSMS_Enable_RFIS { get; private set; }
    public static string pSMS_SMTP_Destinations { get; private set; }
    public static bool pSMS_SMTP_EnableSSL { get; private set; }
    public static string pSMS_SMTP_ServerName { get; private set; }
    public static int pSMS_SMTP_ServerPort { get; private set; }
    public static string pSMS_SMTP_UserName { get; private set; }
    public static string pSMS_SMTP_UserPassword { get; private set; }

    public static string pSQL_ConnectionString_Archive { get; private set; }
    public static string pSQL_ConnectionString_Current { get; private set; }
    public static string pSQL_ConnectionString_Report { get; private set; }


    public static bool pUI_PMIProtection { get; private set; }

    #endregion

    #region Initialize

    private static void Initialize(string commandLine)
    {
        // First assign all properties that are not assigned in the CSE/SSE calls to constants

        pApplicationReleaseDate = "05/06/2022";
        pApplicationVersion = "3.1.4";

        #region DEMO
#if DEMO
        // GRPC
        pGRPC_Endpoint = "https://localhost:443/";

        // CS (Using Phaxio test key/secret)
        pCS_Phaxio_Key = "48b5d0cb315e41a966059b9cd2d109cef8477b73";
        pCS_Phaxio_Secret = "8432b124a868089d72347e0906604a388dd49718";
        pCS_SMTP_EnableSSL = true;
        pCS_SMTP_ServerName = "smtp.office365.com";
        pCS_SMTP_ServerPort = 587;
        //pCS_SMTP_UserName = "OHI_LifeBridgeHealth@outlook.com";
        //pCS_SMTP_UserPassword = "Sidus!optimum!2011";
        pCS_SMTP_UserName = "LifeBridgeHealth@optimiumhealth.com";
        pCS_SMTP_UserPassword = "Sidus!Optimium!2020";

        // FCS
        pFCS_CleanupAge = 30;
        pFCS_CleanupTargets =
            "c:\\Optimiser.ftp\\lci_clinic\\errored;" +
            "c:\\Optimiser.ftp\\lci_clinic\\processed;" +
            "c:\\Optimiser.ftp\\lci_clinic\\test;" +
            "c:\\Optimiser.ftp\\lci_opis\\errored;" +
            "c:\\Optimiser.ftp\\lci_opis\\processed;" +
            "c:\\Optimiser.ftp\\lci_opis\\test;" +
            "c:\\Optimiser.ftp\\riao\\errored;" +
            "c:\\Optimiser.ftp\\riao\\processed;" +
            "c:\\Optimiser.ftp\\surgery\\errored;" +
            "c:\\Optimiser.ftp\\surgery\\processed;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2012.web\\documents;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2012.web\\reports;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2021.web\\documents;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2021.web\\reports;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2021.web.userinterface\\documents;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2021.web.userinterface\\reports;" +
            "c:\\Optimiser.Logs\\Debug\\CommunicationService;" +
            "c:\\Optimiser.Logs\\Debug\\FileCleanupService;" +
            "c:\\Optimiser.Logs\\Debug\\LCIFileInterfaceService;" +
            "c:\\Optimiser.Logs\\Debug\\Optimiser.2012.Web;" +
            "c:\\Optimiser.Logs\\Debug\\Optimiser.2021.Web;" +
            "c:\\Optimiser.Logs\\Debug\\PATFileInterfaceService;" +
            "c:\\Optimiser.Logs\\Debug\\PersistentStorageService;" +
            "c:\\Optimiser.Logs\\Debug\\ReportService;" +
            "c:\\Optimiser.Logs\\Debug\\RIAOFileInterfaceService;" +
            "c:\\Optimiser.Logs\\Debug\\SystemMonitorService;";

        // LFIS
        pLFIS_Enable = false;
        pLFIS_FTPDirectory_Clinic = "";
        pLFIS_FTPDirectory_OPIS = "";
        pLFIS_Log_RawData = false;
        pLFIS_Log_MOID = false;

        // PFIS
        pPFIS_Enable = false;
        pPFIS_FTPDirectory = "";
        pPFIS_Log_RawData = false;
        pPFIS_Log_SPD = false;

        // PSS
        pPSS_ArchivistEnabled = false;
        pPSS_ArchivistSpan = 60;

        pPSS_MaintenanceHour = 21;

        pPSS_ProcessLinkerEnabled = true;
        pPSS_ProcessLinkerUpdateArchive = false;
        pPSS_ProcessLinkerUpdateProcesses = false;

        // RS
        pRS_ReportsDirectory = "c:\\optimiser.2021.Web.UserInterface\\Reports\\";
        pRS_ReportsURLPrefix = "/Reports/";
        pRS_UpdateInfusionData = false;
        pRS_WABAK_Date = new DateTime(2013, 8, 1);
        pRS_WABAK_Enabled = false;
        pRS_WABAK_UpdateDashboards = false;

        // SMS
        pSMS_SystemIdentifier = "MFFL-T570";
        pSMS_Disable0100Report = false;
        pSMS_Enable_LFIS = false;
        pSMS_Enable_PFIS = false;
        pSMS_Enable_RFIS = false;
        pSMS_SMTP_Destinations = "mark@optimiumhealth.com;";
        pSMS_SMTP_EnableSSL = true;
        pSMS_SMTP_ServerName = "smtp.office365.com";
        pSMS_SMTP_ServerPort = 587;
        //pSMS_SMTP_UserName = "OHI_LifeBridgeHealth@outlook.com";
        //pSMS_SMTP_UserPassword = "Sidus!optimum!2011";
        pSMS_SMTP_UserName = "LifeBridgeHealth@optimiumhealth.com";
        pSMS_SMTP_UserPassword = "Sidus!Optimium!2020";

        //SQL
        pSQL_ConnectionString_Archive = "server=localhost;database=Optimiser.2012.LBH.Archive;Trusted_Connection=True";
        pSQL_ConnectionString_Current = "server=localhost;database=Optimiser.2012.LBH.Current;Trusted_Connection=True";
        pSQL_ConnectionString_Report = "server=localhost;database=Optimiser.2012.LBH.Report;Trusted_Connection=True";

        // UI
        pUI_AutoLogoffInterval = 1440;
        pUI_PMIProtection = true;

        // WEB (These are settings used in the Web.DataServer Services that aren't
        // persistent)
        pWEB_DocumentsDirectory = "c:\\optimiser.2021.web.userinterface\\Documents\\";
        pWEB_DocumentsURLPrefix = "/Documents/";

#endif
        #endregion

        #region DEVELOP
#if DEVELOP

        // GRPC
        pGRPC_Endpoint = "https://localhost:44350/";

        // CS (Using Phaxio test key/secret)
        pCS_Phaxio_Key = "48b5d0cb315e41a966059b9cd2d109cef8477b73";
        pCS_Phaxio_Secret = "8432b124a868089d72347e0906604a388dd49718";
        pCS_SMTP_EnableSSL = true;
        pCS_SMTP_ServerName = "smtp.office365.com";
        pCS_SMTP_ServerPort = 587;
        //pCS_SMTP_UserName = "OHI_LifeBridgeHealth@outlook.com";
        //pCS_SMTP_UserPassword = "Sidus!optimum!2011";
        pCS_SMTP_UserName = "LifeBridgeHealth@optimiumhealth.com";
        pCS_SMTP_UserPassword = "Sidus!Optimium!2020";

        // FCS
        pFCS_CleanupAge = 30;
        pFCS_CleanupTargets =
            "c:\\Optimiser.ftp\\lci_clinic\\errored;" +
            "c:\\Optimiser.ftp\\lci_clinic\\processed;" +
            "c:\\Optimiser.ftp\\lci_opis\\errored;" +
            "c:\\Optimiser.ftp\\lci_opis\\processed;" +
            "c:\\Optimiser.ftp\\riao\\errored;" +
            "c:\\Optimiser.ftp\\riao\\processed;" +
            "c:\\Optimiser.ftp\\surgery\\errored;" +
            "c:\\Optimiser.ftp\\surgery\\processed;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2012.web\\documents;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2012.web\\reports;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2021.web\\documents;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2021.web\\reports;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2021.web.userinterface\\documents;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2021.web.userinterface\\reports;" +
            "c:\\Optimiser.Logs\\Debug\\CommunicationService;" +
            "c:\\Optimiser.Logs\\Debug\\FileCleanupService;" +
            "c:\\Optimiser.Logs\\Debug\\LCIFileInterfaceService;" +
            "c:\\Optimiser.Logs\\Debug\\Optimiser.2012.Web;" +
            "c:\\Optimiser.Logs\\Debug\\Optimiser.2021.Web;" +
            "c:\\Optimiser.Logs\\Debug\\PATFileInterfaceService;" +
            "c:\\Optimiser.Logs\\Debug\\PersistentStorageService;" +
            "c:\\Optimiser.Logs\\Debug\\ReportService;" +
            "c:\\Optimiser.Logs\\Debug\\RIAOFileInterfaceService;" +
            "c:\\Optimiser.Logs\\Debug\\SystemMonitorService;";

        // LFIS
        pLFIS_Enable = false;
        pLFIS_FTPDirectory_Clinic = "c:\\optimiser.ftp\\lci_clinic";
        pLFIS_FTPDirectory_OPIS = "c:\\optimiser.ftp\\lci_opis";
        pLFIS_Log_RawData = false;
        pLFIS_Log_MOID = false;

        // PFIS
        pPFIS_Enable = false;
        pPFIS_FTPDirectory = "c:\\optimiser.ftp\\surgery";
        pPFIS_Log_RawData = false;
        pPFIS_Log_SPD = false;

        // PSS
        pPSS_ArchivistEnabled = true;
        pPSS_ArchivistSpan = 90;

        pPSS_MaintenanceHour = 21;

        pPSS_ProcessLinkerEnabled = true;
        pPSS_ProcessLinkerUpdateArchive = false;
        pPSS_ProcessLinkerUpdateProcesses = false;

        // RS
        pRS_ReportsDirectory = "c:\\solutions\\ohi\\optimiser\\optimiser.2021.web.userinterface\\Reports\\";
        pRS_ReportsURLPrefix = "/Reports/";
        pRS_UpdateInfusionData = false;
        pRS_WABAK_Date = new DateTime(2013, 8, 1);
        pRS_WABAK_Enabled = false;
        pRS_WABAK_UpdateDashboards = false;

        // SMS
        pSMS_SystemIdentifier = "MFFL-T570 [O.2021]";
        pSMS_Disable0100Report = false;
        pSMS_Enable_LFIS = false;
        pSMS_Enable_PFIS = false;
        pSMS_Enable_RFIS = false;
        pSMS_SMTP_Destinations = "mark@optimiumhealth.com;";
        pSMS_SMTP_EnableSSL = true;
        pSMS_SMTP_ServerName = "smtp.office365.com";
        pSMS_SMTP_ServerPort = 587;
        //pSMS_SMTP_UserName = "OHI_LifeBridgeHealth@outlook.com";
        //pSMS_SMTP_UserPassword = "Sidus!optimum!2011";
        pSMS_SMTP_UserName = "LifeBridgeHealth@optimiumhealth.com";
        pSMS_SMTP_UserPassword = "Sidus!Optimium!2020";

        // SQL
        pSQL_ConnectionString_Archive = "server=localhost;database=Optimiser.2012.LBH.Archive;Trusted_Connection=True";
        pSQL_ConnectionString_Current = "server=localhost;database=Optimiser.2012.LBH.Current;Trusted_Connection=True";
        pSQL_ConnectionString_Report = "server=localhost;database=Optimiser.2012.LBH.Report;Trusted_Connection=True";

        // UI
        pUI_AutoLogoffInterval = 1440;
        pUI_PMIProtection = false;

        // WEB (These are settings used in the Web.DataServer Services that aren't
        // persistent)
        pWEB_DocumentsDirectory = "c:\\solutions\\ohi\\optimiser\\optimiser.2021.web.userinterface\\Documents\\";
        pWEB_DocumentsURLPrefix = "/Documents/";

#endif
        #endregion

        #region LBH
#if LBH

        // GRPC
        pGRPC_Endpoint = "https://SHBOPTIMISER:444/";

        // CS (Using Phaxio LBH key/secret)
        pCS_Phaxio_Key = "bf68b295e4fa00547e610f4377b1d0d11f3a42ea";
        pCS_Phaxio_Secret = "da1373828b7dc763c2542b445b0dfb53cc663e71";
        pCS_SMTP_EnableSSL = true;
        pCS_SMTP_ServerName = "smtp.office365.com";
        pCS_SMTP_ServerPort = 587;
        //pCS_SMTP_UserName = "OHI_LifeBridgeHealth@outlook.com";
        //pCS_SMTP_UserPassword = "Sidus!optimum!2011";
        pCS_SMTP_UserName = "LifeBridgeHealth@optimiumhealth.com";
        pCS_SMTP_UserPassword = "Sidus!Optimium!2020";

        // FCS
        pFCS_CleanupAge = 90;
        pFCS_CleanupTargets =
            "e:\\ftp;" +
            "e:\\ftp\\lci_clinic\\errored;" +
            "e:\\ftp\\lci_clinic\\processed;" +
            "e:\\ftp\\lci_clinic\\test;" +
            "e:\\ftp\\lci_opis\\errored;" +
            "e:\\ftp\\lci_opis\\processed;" +
            "e:\\ftp\\lci_opis\\test;" +
            "e:\\ftp\\surgery\\errored;" +
            "e:\\ftp\\surgery\\processed;" +
            "e:\\ftp\\surgery\\test;" +
            "e:\\ftp\\riao;" +
            "e:\\ftp\\riao\\errored;" +
            "e:\\ftp\\riao\\processed;" +
            "e:\\ftp\\riao\\test;" +
            "e:\\Optimiser.2012\\documents;" +
            "e:\\Optimiser.2012\\reports;" +
            "e:\\Optimiser.2021\\documents;" +
            "e:\\Optimiser.2021\\reports;" +
            "e:\\Optimiser.Logs\\LBH\\CommunicationService;" +
            "e:\\Optimiser.Logs\\LBH\\FileCleanupService;" +
            "e:\\Optimiser.Logs\\LBH\\LCIFileInterfaceService;" +
            "e:\\Optimiser.Logs\\LBH\\Optimiser.2012.Web;" +
            "e:\\Optimiser.Logs\\LBH\\Optimiser.2021.Web;" +
            "e:\\Optimiser.Logs\\LBH\\PATFileInterfaceService;" +
            "e:\\Optimiser.Logs\\LBH\\PersistentStorageService;" +
            "e:\\Optimiser.Logs\\LBH\\ReportService;" +
            "e:\\Optimiser.Logs\\LBH\\RIAOFileInterfaceService;" +
            "e:\\Optimiser.Logs\\LBH\\SystemMonitorService;";

        // LFIS
        pLFIS_Enable = true;
        pLFIS_FTPDirectory_Clinic = "e:\\ftp\\lci_clinic";
        pLFIS_FTPDirectory_OPIS = "e:\\ftp\\lci_opis";
        pLFIS_Log_RawData = false;
        pLFIS_Log_MOID = false;

        // PFIS
        pPFIS_Enable = true;
        pPFIS_FTPDirectory = "e:\\ftp\\surgery";
        pPFIS_Log_RawData = false;
        pPFIS_Log_SPD = false;

        // PSS
        pPSS_ArchivistEnabled = true;
        pPSS_ArchivistSpan = 90;

        pPSS_MaintenanceHour = 21;

        pPSS_ProcessLinkerEnabled = false;
        pPSS_ProcessLinkerUpdateArchive = false;
        pPSS_ProcessLinkerUpdateProcesses = false;

        // RS
        pRS_ReportsDirectory = "e:\\optimiser.2021\\Reports\\";
        pRS_ReportsURLPrefix = "/Reports/";
        pRS_UpdateInfusionData = false;
        pRS_WABAK_Date = new DateTime(2013, 8, 1);
        pRS_WABAK_Enabled = false;
        pRS_WABAK_UpdateDashboards = false;

        // SMS
        pSMS_SystemIdentifier = "SHB-OPTIMISER";
        pSMS_Disable0100Report = true;
        pSMS_Enable_LFIS = true;
        pSMS_Enable_PFIS = true;
        pSMS_Enable_RFIS = false;
        pSMS_SMTP_Destinations = 
            "adayucos@lifebridgehealth.org;" +
            "gbrocket@lifebridgehealth.org;" +
            "mark@optimiumhealth.com;" +
            "paprice@lifebridgehealth.org;" +
            "patjohns@lifebridgehealth.org;" +
            "PowerInsightSupport@lifebridgehealth.org;" +
            "tasingle@lifebridgehealth.org;" ;

        pSMS_SMTP_EnableSSL = true;
        pSMS_SMTP_ServerName = "smtp.office365.com";
        pSMS_SMTP_ServerPort = 587;
        //pSMS_SMTP_UserName = "OHI_LifeBridgeHealth@outlook.com";
        //pSMS_SMTP_UserPassword = "Sidus!optimum!2011";
        pSMS_SMTP_UserName = "LifeBridgeHealth@optimiumhealth.com";
        pSMS_SMTP_UserPassword = "Sidus!Optimium!2020";

        // SQL
        pSQL_ConnectionString_Archive = "server=localhost\\SQLEXPRESS;database=Optimiser.2012.LBH.Archive;Trusted_Connection=True";
        pSQL_ConnectionString_Current = "server=localhost\\SQLEXPRESS;database=Optimiser.2012.LBH.Current;Trusted_Connection=True";
        pSQL_ConnectionString_Report = "server=localhost\\SQLEXPRESS;database=Optimiser.2012.LBH.Report;Trusted_Connection=True";

        // UI
        pUI_AutoLogoffInterval = 60;
        pUI_PMIProtection = false;

        // WEB (These are settings used in the Web.DataServer Services that aren't
        // persistent)
        pWEB_DocumentsDirectory = "e:\\optimiser.2021\\Documents\\";
        pWEB_DocumentsURLPrefix = "/Documents/";

#endif
        #endregion

        #region WASM
#if WASM

        // GRPC
        pGRPC_Endpoint = "https://localhost:44350/";

        // CS (Using Phaxio test key/secret)
        pCS_Phaxio_Key = "48b5d0cb315e41a966059b9cd2d109cef8477b73";
        pCS_Phaxio_Secret = "8432b124a868089d72347e0906604a388dd49718";
        pCS_SMTP_EnableSSL = true;
        pCS_SMTP_ServerName = "smtp.office365.com";
        pCS_SMTP_ServerPort = 587;
        //pCS_SMTP_UserName = "OHI_LifeBridgeHealth@outlook.com";
        //pCS_SMTP_UserPassword = "Sidus!optimum!2011";
        pCS_SMTP_UserName = "LifeBridgeHealth@optimiumhealth.com";
        pCS_SMTP_UserPassword = "Sidus!Optimium!2020";

        // FCS
        pFCS_CleanupAge = 30;
        pFCS_CleanupTargets =
            "c:\\Optimiser.ftp\\lci_clinic\\errored;" +
            "c:\\Optimiser.ftp\\lci_clinic\\processed;" +
            "c:\\Optimiser.ftp\\lci_clinic\\test;" +
            "c:\\Optimiser.ftp\\lci_opis\\errored;" +
            "c:\\Optimiser.ftp\\lci_opis\\processed;" +
            "c:\\Optimiser.ftp\\lci_opis\\test;" +
            "c:\\Optimiser.ftp\\riao\\errored;" +
            "c:\\Optimiser.ftp\\riao\\processed;" +
            "c:\\Optimiser.ftp\\surgery\\errored;" +
            "c:\\Optimiser.ftp\\surgery\\processed;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2012.web\\documents;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2012.web\\reports;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2021.web\\documents;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2021.web\\reports;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2021.web.userinterface\\documents;" +
            "c:\\Solutions\\OHI\\Optimiser\\Optimiser.2021.web.userinterface\\reports;" +
            "c:\\Optimiser.Logs\\Debug\\CommunicationService;" +
            "c:\\Optimiser.Logs\\Debug\\FileCleanupService;" +
            "c:\\Optimiser.Logs\\Debug\\LCIFileInterfaceService;" +
            "c:\\Optimiser.Logs\\Debug\\Optimiser.2012.Web;" +
            "c:\\Optimiser.Logs\\Debug\\Optimiser.2021.Web;" +
            "c:\\Optimiser.Logs\\Debug\\PATFileInterfaceService;" +
            "c:\\Optimiser.Logs\\Debug\\PersistentStorageService;" +
            "c:\\Optimiser.Logs\\Debug\\ReportService;" +
            "c:\\Optimiser.Logs\\Debug\\RIAOFileInterfaceService;" +
            "c:\\Optimiser.Logs\\Debug\\SystemMonitorService;";

        // LFIS
        pLFIS_Enable = false;
        pLFIS_FTPDirectory_Clinic = "";
        pLFIS_FTPDirectory_OPIS = "";
        pLFIS_Log_RawData = false;
        pLFIS_Log_MOID = false;

        // PFIS
        pPFIS_Enable = false;
        pPFIS_FTPDirectory = "";
        pPFIS_Log_RawData = false;
        pPFIS_Log_SPD = false;

        // PSS
        pPSS_ArchivistEnabled = true;
        pPSS_ArchivistSpan = 90;

        pPSS_MaintenanceHour = 21;

        pPSS_ProcessLinkerEnabled = true;
        pPSS_ProcessLinkerUpdateArchive = false;
        pPSS_ProcessLinkerUpdateProcesses = false;

        // RS
        pRS_ReportsDirectory = "c:\\optimiser.2021.Web.UserInterface\\Reports\\";
        pRS_ReportsURLPrefix = "/Reports/";
        pRS_UpdateInfusionData = false;
        pRS_WABAK_Date = new DateTime(2013, 8, 1);
        pRS_WABAK_Enabled = false;
        pRS_WABAK_UpdateDashboards = false;

        // SMS
        pSMS_SystemIdentifier = "MFFL-T570";
        pSMS_Disable0100Report = false;
        pSMS_Enable_LFIS = false;
        pSMS_Enable_PFIS = false;
        pSMS_Enable_RFIS = false;
        pSMS_SMTP_Destinations = "mark@optimiumhealth.com;";
        pSMS_SMTP_EnableSSL = true;
        pSMS_SMTP_ServerName = "smtp.office365.com";
        pSMS_SMTP_ServerPort = 587;
        //pSMS_SMTP_UserName = "OHI_LifeBridgeHealth@outlook.com";
        //pSMS_SMTP_UserPassword = "Sidus!optimum!2011";
        pSMS_SMTP_UserName = "LifeBridgeHealth@optimiumhealth.com";
        pSMS_SMTP_UserPassword = "Sidus!Optimium!2020";

        // SQL
        pSQL_ConnectionString_Archive = "server=localhost;database=Optimiser.2012.LBH.Archive;Trusted_Connection=True";
        pSQL_ConnectionString_Current = "server=localhost;database=Optimiser.2012.LBH.Current;Trusted_Connection=True";
        pSQL_ConnectionString_Report = "server=localhost;database=Optimiser.2012.LBH.Report;Trusted_Connection=True";

        // UI
        pUI_AutoLogoffInterval = 1440;
        pUI_PMIProtection = true;

        // WEB (These are settings used in the Web.DataServer Services that aren't
        // persistent)
        pWEB_DocumentsDirectory = "c:\\optimiser.2021.Web.UserInterface\\Documents\\";
        pWEB_DocumentsURLPrefix = "/Documents/";

#endif
        #endregion

        // Override properties defined in p_CommandLine

    }

    //
    //  If we need to do different intialization based on CSE/SSE these two helpers are defined.
    //  At the moment there are no differences.
    //
    public static void InitializeCSE(string commandLine)
    {
        Initialize(commandLine);
    }

    public static void InitializeSSE(string commandLine)
    {
        Initialize(commandLine);
    }

    #endregion
}

