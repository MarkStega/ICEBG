using ICEBG.AppConfig;
using ICEBG.DataTier.DataDefinitions;
using ICEBG.DataTier.HelperClasses;

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

    private string count1 { get; set; } = "'Not yet initialized'";
    private string count2 { get; set; } = "0";
    private string duration { get; set; } = "'Not yet initialized'";
    private DateTime startTime { get; set; } = DateTime.MinValue;
    private string time { get; set; } = "'Not yet initialized'";
    private ServiceResult<Configuration_DD> configuration { get; set; }

    private int internalCount = 0;
    private int internalCount2 = 0;

    #region LoadReportCollectionAsync
    private async Task LoadReportCollectionAsync()
    {
        configuration = await ConfigurationClient.SelectAsync(ApplicationConfiguration.pConfigurationIdentifier);
        internalCount += 1;
        count1 = internalCount.ToString("N0");

        var currentTime = DateTime.Now;
        if (startTime == DateTime.MinValue)
        {
            startTime = currentTime;
        }

        duration = (currentTime - startTime).ToString(@"d\.hh\:mm\:ss");

        time = currentTime.ToString();
        if (internalCount >= 1000000)
        {
            internalCount = 0;
            internalCount2 += 1;
            count2 = internalCount2.ToString("N0");
        }
        StateHasChanged();
        pTimer?.Dispose();
        pTimer = new System.Timers.Timer(10);
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

