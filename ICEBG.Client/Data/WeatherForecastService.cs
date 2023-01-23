using System;
using System.Net.Http.Json;
using System.Threading.Tasks;

using ICEBG.AppConfig;
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
                request.RequestUri = new Uri(ApplicationConfiguration.pDataServicesEndpointPrefix + ApplicationConfiguration.pWeatherEndpoint);
                var response = await client.SendAsync(request);
                var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
                return forecasts;
            }
        }
    }
}

//[2022-10-19T21:27:27.516Z] Error: System.Net.Http.HttpRequestException: The SSL connection could not be established, see inner exception.
//--->System.Security.Authentication.AuthenticationException: The remote certificate is invalid because of errors in the certificate chain: NotTimeValid