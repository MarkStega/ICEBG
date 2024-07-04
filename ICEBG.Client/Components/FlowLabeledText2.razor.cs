using ICEBG.AppConfig;
using ICEBG.DataTier.DataDefinitions;
using ICEBG.DataTier.HelperClasses;

using Microsoft.AspNetCore.Components;

namespace ICEBG.Client.Components;

public partial class FlowLabeledText2
{
    [Parameter] public string duration { get; set; }
    [Parameter] public string time { get; set; }
    [Parameter] public ServiceResult<StatisticsReport_DD> statisticsReportResult { get; set; }
}

