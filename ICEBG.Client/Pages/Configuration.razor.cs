using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ICEBG.DataTier.DataDefinitions;
using ICEBG.DataTier.HelperClasses;
using ICEBG.SystemFramework;

using Microsoft.AspNetCore.Components;

namespace ICEBG.Client.Pages;

[Sitemap(SitemapAttribute.eChangeFreqType.Weekly, 0.8)]
public partial class Configuration : ComponentBase
{
    private ServiceResult<Configuration_DD> configuration;

    protected override async Task OnInitializedAsync()
    {
        configuration = await ConfigurationClient.SelectAsync(ApplicationConfiguration.pConfigurationIdentifier);
    }
}
