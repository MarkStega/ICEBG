using ICEBG.DataTier.DataDefinitions;
using ICEBG.DataTier.gRPCClient;
using ICEBG.DataTier.HelperClasses;
using ICEBG.DataTier.Interfaces;

using Microsoft.AspNetCore.Components;

using System;
using System.Threading.Tasks;
using System.Timers;

namespace ICEBG.Client;

/// <summary>
/// The website's index page
/// </summary>
[Sitemap(SitemapAttribute.eChangeFreqType.Weekly, 0.8)]
public partial class Index : ComponentBase
{
#if BURN
    private bool pBurn { get; set; } = true;
#else
    private bool pBurn { get; set; } = false;
#endif

    private System.Timers.Timer pTimer { get; set; }
    private DateTime currentTime { get; set; } = DateTime.MinValue;
    private string duration { get; set; } = "'Not yet initialized'";
    private DateTime previousApiTime { get; set; } = DateTime.MinValue;
    private DateTime startTime { get; set; } = DateTime.MinValue;
    private ServiceResult<StatisticsReport_DD> currentStatisticsReport { get; set; }

    #region LoadReportCollectionAsync
    private async Task LoadReportCollectionAsync()
    {
        currentTime = DateTime.Now;
        if (startTime == DateTime.MinValue)
        {
            startTime = currentTime;
        }

        if ((currentTime - previousApiTime) > new TimeSpan(0,1,0))
        {
            currentStatisticsReport = await ConfigurationClient.StatisticsReportAsync();
            previousApiTime = currentTime;
        }

        duration = (currentTime - startTime).ToString(@"d\.hh\:mm\:ss");

        StateHasChanged();
        pTimer?.Dispose();
        pTimer = new System.Timers.Timer(500);
        pTimer.Elapsed += RefreshTimerTick;
        pTimer.Enabled = true;
    }

    #endregion

    protected override void OnAfterRender(bool isFirstRender)
    {
        if (isFirstRender)
        {
#if BURN
            RefreshTimerTick(null, null);
#endif
        }
    }

    #region RefreshTimerTick
    public void RefreshTimerTick(Object source, ElapsedEventArgs e)
    {
        InvokeAsync(LoadReportCollectionAsync);
    }
    #endregion

    #region IDispose
    void IDisposable.Dispose()
    {
        pTimer?.Dispose();
    }

    #endregion


}

