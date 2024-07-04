using ICEBG.AppConfig;
using ICEBG.DataTier.Interfaces;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

//
//  2024-07-04  Mark Stega
//              Created
//

namespace ICEBG.Web.DataServices;

partial class ConfigurationServiceWorker
{
    public async Task ManipulateSqlDataWorker()
    {
        pLogger.Log(Microsoft.Extensions.Logging.LogLevel.Debug, "ConfigurationServiceWorker.ManipulateSqlDataWorker() was started.");

        await Task.Yield();

        while (!pStartWorkerThreads)
        {
            Thread.Sleep(5000);
        }

        pLogger.Log(Microsoft.Extensions.Logging.LogLevel.Debug, "ConfigurationServiceWorker.ManipulateSqlDataWorker() released for work");

        pSqlStartTime = DateTime.Now;

        var spansSum = new TimeSpan(0);

        while (true)
        {
            try
            {
                ManipulateSqlData(1_000_000, out TimeSpan ts);

                pSqlHeartbeat = DateTime.Now;
                spansSum += ts;
                pSqlIterations++;
                pSqlAverageSpan = new TimeSpan(spansSum.Ticks / pSqlIterations);

                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                pLogger.Log(LogLevel.Error, "Exception in ConfigurationServiceWorker.ManipulateSqlDataWorker of '" + ex.ToString() + "'");
            }
        }
    }

    private void ManipulateSqlData(int iterations, out TimeSpan ts)
    {
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        var configuration = pConfigurationBL.Select(ApplicationConfiguration.pConfigurationIdentifier);
        var now = DateTime.Now;
        for (int f = 1; f <= iterations; f++)
        {
            configuration.Id = f.ToString();
            configuration.Configuration = "ConfigurationValue: " + now;
            configuration.ServerVersion = "ServerVersion: " + now;
            pConfigurationBL.Upsert(configuration);
        }


        stopwatch.Stop();
        ts = stopwatch.Elapsed;
    }

}

