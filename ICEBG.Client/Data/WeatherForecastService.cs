using System;
using System.Net.Http.Json;
using System.Threading.Tasks;

using ICEBG.SystemFramework;

namespace ICEBG.Client
{
    public class WeatherForecastService
    {

        public async Task<WeatherForecast[]?> GetForecastAsync()
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var request = new System.Net.Http.HttpRequestMessage();
                request.RequestUri = new Uri(ApplicationConfiguration.pWeatherEndpoint);
                var response = await client.SendAsync(request);
                var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
                return forecasts;
            }
        }
    }
}