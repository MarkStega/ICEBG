using System;
using System.Collections.Generic;
using System.Linq;
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
        private static System.Timers.Timer pTimer { get; set; }

        private string time { get; set; } = "'Not yet initialized'";
        private ServiceResult<Configuration_DD> configuration { get; set; }

        #region LoadReportCollectionAsync
        private async Task LoadReportCollectionAsync()
        {
            for (int i = 0; i < 100; i++)
            {
                configuration = await ConfigurationClient.SelectAsync(ApplicationConfiguration.pConfigurationIdentifier);
            }
            time = DateTime.Now.ToString();
            StateHasChanged();
            pTimer.Dispose();
            pTimer = new System.Timers.Timer(100);
            pTimer.Elapsed += RefreshTimerTick;
            pTimer.Enabled = true;
        }

        #endregion

        protected override void OnInitialized()
        {
            pTimer = new System.Timers.Timer(100);
            pTimer.Elapsed += RefreshTimerTick;
            pTimer.Enabled = true;
        }

        #region RefreshTimerTick
        public void RefreshTimerTick(Object source, ElapsedEventArgs e)
        {
            InvokeAsync(LoadReportCollectionAsync);
        }
        #endregion

    }
}
