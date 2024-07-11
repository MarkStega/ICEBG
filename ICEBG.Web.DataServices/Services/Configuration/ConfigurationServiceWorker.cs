using System;
using System.Threading;
using System.Threading.Tasks;

using ICEBG.AppConfig;
using ICEBG.DataTier.BusinessLogic;
using ICEBG.SystemFramework;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NLog;

//
//  2024-07-04  Mark Stega
//              Created
//

namespace ICEBG.Web.DataServices;


public partial class ConfigurationServiceWorker : BackgroundService
{
    #region members

    // ComputePiWorker stats
    public TimeSpan pPiAverageSpan { get; set; } = new TimeSpan(0);
    public DateTime pPiStartTime { get; set; }
    public DateTime pPiHeartbeat { get; set; }
    public int pPiIterations { get; set; } = 0;

    // ManipulateSqlDataWorker stats
    public TimeSpan pSqlAverageSpan { get; set; } = new TimeSpan(0);
    public DateTime pSqlStartTime { get; set; }
    public DateTime pSqlHeartbeat { get; set; }
    public int pSqlIterations { get; set; } = 0;

    // Miscellaneous
    protected IConfiguration pConfiguration { get; private set; }
    protected Configuration_BL pConfigurationBL { get; private set; }
    protected ILogger<LoggingFramework> pLogger { get; private set; }
    private bool pStartWorkerThreads { get; set; } = false;

    #endregion

    #region ctor
    public ConfigurationServiceWorker(
    IConfiguration configuration,
        ILogger<LoggingFramework> logger)
    {
        pConfiguration = configuration;
        pConfigurationBL = new Configuration_BL(ApplicationConfiguration.pSqlConnectionString);
        pLogger = logger;
        pLogger.LogDebug("ConfigurationServiceWorker ctor()");

        WorkerServiceReference.pConfigurationWorkerServiceReference = this;
    }
    #endregion

    #region ExecuteAsync
    static bool kInitialized = false;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                if (!kInitialized)
                {
                    kInitialized = true;

                    pLogger.Log(Microsoft.Extensions.Logging.LogLevel.Debug, "ConfigurationServiceWorker.ExecuteAsync starting ComputePiWorker()");
                    ComputePiWorker();

                    pLogger.Log(Microsoft.Extensions.Logging.LogLevel.Debug, "ConfigurationServiceWorker.ExecuteAsync starting ManipulateSqlDataWorker()");
                    ManipulateSqlDataWorker();

                    pLogger.Log(Microsoft.Extensions.Logging.LogLevel.Debug, "ConfigurationServiceWorker.ExecuteAsync sleeping for 1 minute before allowing worker threads to start");
                    await Task.Delay(1000 * 60); // 1 minute
                    pStartWorkerThreads = true;
                    pLogger.Log(Microsoft.Extensions.Logging.LogLevel.Debug, "ConfigurationServiceWorker.ExecuteAsync released worker threads");
                }
                await Task.Yield();
            }
        }
        catch (OperationCanceledException)
        {
            // Do nothing, this is expected upon service shutdown
        }
        catch (Exception ex)
        {
            pLogger.Log(Microsoft.Extensions.Logging.LogLevel.Error, "ConfigurationServiceWorker.ExecuteAsync caught exception " + ex.ToString());
        }
    }

    #endregion
}


#region resource log

// 2024-07-04 1135 -  $4,737
// 2024-07-05 0707 -  $4,770 - Created aseICEBG-BURN-1,
//                             Created waICEBG-BURN-1,
//                             Deployed waICEBG-BURN-1, not running
// 2024-07-06 0745 -  $5,450 - waICEBG-BURN-1 running, not displaying stats,
//                             aseICEBG-BURN-2 creation started
// 2024-07-07 0645 -  $6,369 - waICEBG-BURN-1 running, not displaying stats,
//                             aseICEBG-BURN-2 creation failure reported & restarted
// 2024-07-08 0600 -  $7,316 - waICEBG-BURN-1 running, not displaying stats,
//                             aseICEBG-BURN-2 creation failure reported,
//                             Created aseICEBG-BURN-3,
//                             Created waICEBG-BURN-3,
//                             Deployed waICEBG-BURN-3
// 2024-07-09 0630 -  $9,062 - waICEBG-BURN-1 running, not displaying stats,
//                             waICEBG-BURN-3 running, not displaying stats
// 2024-07-10 1330 - $11,401 - waICEBG-BURN-1 running, not displaying stats,
//                             waICEBG-BURN-3 running, not displaying stats
// 2024-07-11 0800 - $12,797 - waICEBG-BURN-1 running, not displaying stats,
//                             waICEBG-BURN-3 running, not displaying stats

#endregion
