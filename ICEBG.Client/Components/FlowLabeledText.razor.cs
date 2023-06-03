using ICEBG.AppConfig;
using ICEBG.DataTier.DataDefinitions;
using ICEBG.DataTier.HelperClasses;

using Microsoft.AspNetCore.Components;

namespace ICEBG.Client.Components;

public partial class FlowLabeledText
{
    [Parameter] public string time { get; set; }
    [Parameter] public ServiceResult<Configuration_DD> configuration { get; set; }
}

