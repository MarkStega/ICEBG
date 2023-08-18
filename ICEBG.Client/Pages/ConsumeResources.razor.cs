using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using ICEBG.AppConfig;
using ICEBG.DataTier.DataDefinitions;
using ICEBG.DataTier.HelperClasses;
using ICEBG.DataTier.Interfaces;

using Material.Blazor;

using Microsoft.AspNetCore.Components;

namespace ICEBG.Client.Pages
{
    public partial class ConsumeResources : ComponentBase
    {
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
            if (internalCount >= (int.MaxValue - 100))
            {
                internalCount = 0;
                internalCount2 += 1;
                count2 = internalCount2.ToString("N0");
            }
            StateHasChanged();
            pTimer?.Dispose();
            pTimer = new System.Timers.Timer(1000 * 60 * 60);
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
}
