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
                // Call *mywebapi*, and display its response in the page
                var request = new System.Net.Http.HttpRequestMessage();
                // webapi is the container name
                request.RequestUri = new Uri(ApplicationConfiguration.pWeatherEndpoint);
                var response = await client.SendAsync(request);
                var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
                return forecasts;
            }
        }
    }
}