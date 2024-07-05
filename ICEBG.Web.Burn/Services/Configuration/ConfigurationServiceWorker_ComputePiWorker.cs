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
    public async Task ComputePiWorker()
    {
        pLogger.LogDebug("ConfigurationServiceWorker.ComputePiWorker() was started.");

        await Task.Yield();

        while (!pStartWorkerThreads)
        {
            Thread.Sleep(5000);
        }

        pLogger.LogDebug("ConfigurationServiceWorker.ComputePiWorker() released for work");

        pPiStartTime = DateTime.Now;

        var spansSum = new TimeSpan(0);

        while (true)
        {
            try
            {
                EstimatePiWithNilakantha(100_000_000, out decimal nilakanthaPi, out TimeSpan ts);

                pPiHeartbeat = DateTime.Now;
                spansSum += ts;
                pPiIterations++;
                pPiAverageSpan = new TimeSpan(spansSum.Ticks / pPiIterations);

                pLogger.LogDebug("ConfigurationServiceWorker.ComputePiWorker completed iteration '" + pPiIterations.ToString() + "'");

                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                pLogger.LogError("Exception in ConfigurationServiceWorker.ComputePiWorker of '" + ex.ToString() + "'");
            }
        }
    }

    private void EstimatePiWithNilakantha(int iterations, out decimal nilakanthaPi, out TimeSpan ts)
    {
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        // Nilakantha formula - code written completely by Todd Mandell 2014
        // π = 3 + 4/(2*3*4) - 4/(4*5*6) + 4/(6*7*8) - 4/(8*9*10) + 4/(10*11*12) - (4/(12*13*14) etc

        nilakanthaPi = 0;
        decimal a = 2;
        decimal b = 3;
        decimal c = 4;

        for (int f = 1; f <= iterations; f++)
        {

            nilakanthaPi += 4 / (a * b * c);

            a += 2;
            b += 2;
            c += 2;

            nilakanthaPi -= 4 / (a * b * c);

            a += 2;
            b += 2;
            c += 2;
        }

        nilakanthaPi += 3;

        stopwatch.Stop();
        ts = stopwatch.Elapsed;
    }

}

