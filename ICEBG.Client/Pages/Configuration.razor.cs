using System.Threading.Tasks;

using ICEBG.AppConfig;
using ICEBG.DataTier.DataDefinitions;
using ICEBG.DataTier.HelperClasses;

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
