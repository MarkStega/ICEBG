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
        pLogger.LogInformation("ConfigurationServiceWorker ctor()");

        pPiHeartbeat = DateTime.Now;

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

