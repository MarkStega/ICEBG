using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

namespace ICEBG.Client;

[Sitemap(SitemapAttribute.eChangeFreqType.Weekly, 0.8)]
public partial class Weather : ComponentBase
{
    [Inject] WeatherForecastService pForecastService { get; set; }

#nullable enable

    private WeatherForecast[]? forecasts;
    protected override async Task OnInitializedAsync()
    {
        forecasts = await pForecastService.GetForecastAsync();
    }
}
