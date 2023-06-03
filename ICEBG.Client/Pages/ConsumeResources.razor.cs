﻿using System;
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
        private static System.Timers.Timer pTimer { get; set; }

        private string count { get; set; } = "'Not yet initialized'";
        private string count2 { get; set; } = "'Not yet initialized'";
        private string time { get; set; } = "'Not yet initialized'";
        private ServiceResult<Configuration_DD> configuration { get; set; }

        private int internalCount = 0;
        private int internalCount2 = 0;

        #region LoadReportCollectionAsync
        private async Task LoadReportCollectionAsync()
        {
            for (int i = 0; i < 10; i++)
            {
                configuration = await ConfigurationClient.SelectAsync(ApplicationConfiguration.pConfigurationIdentifier);
                internalCount += 1;
                count = internalCount.ToString();
            }
            time = DateTime.Now.ToString();
            if (internalCount >=32000)
            {
                internalCount = 0;
                internalCount2 += 1;
                count2 = internalCount2.ToString();
            }
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