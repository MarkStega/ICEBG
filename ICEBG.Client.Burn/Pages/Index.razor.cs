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
    [CascadingParameter] private MainLayout MainLayout { get; set; } = default!;


    private class ImageData
    {
        public string Uri { get; set; } = "";
        public string Caption { get; set; } = "";
        public string Width { get; set; } = "";
        public string Height { get; set; } = "";
        public bool Preload { get; set; } = true;
        public string Rel => Preload ? "preload" : "prefetch";
    }


    [Inject] private NavigationManager NavigationManager { get; set; } = default!;


    private static readonly ImageData[] SkylineImages = new ImageData[]
    {
        new() { Uri = "_content/ICEBG.Client/images/new-york-640.webp", Caption = "New York skyline", Width = "640px", Height = "420px" },
        new() { Uri = "_content/ICEBG.Client/images/new-york-420.webp", Caption = "New York skyline", Width = "420px", Height = "420px" },
        new() { Uri = "_content/ICEBG.Client/images/new-york-320.webp", Caption = "New York skyline", Width = "320px", Height = "420px" },
    };


    private static readonly ImageData[] ProgrammerImages = new ImageData[]
    {
        new() { Uri = "_content/ICEBG.Client/images/programmer-640.webp", Caption = "Programmer working at a desk", Width = "640px", Height = "420px" },
        new() { Uri = "_content/ICEBG.Client/images/programmer-420.webp", Caption = "Programmer working at a desk", Width = "420px", Height = "420px" },
        new() { Uri = "_content/ICEBG.Client/images/programmer-320.webp", Caption = "Programmer working at a desk", Width = "320px", Height = "420px" },
    };

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
            RefreshTimerTick(null, null);
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

