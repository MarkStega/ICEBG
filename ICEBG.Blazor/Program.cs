//
// ICEBG.Blazor
//
//  2022-11-09  Mark Stega
//              Created
//

using System;
using System.Net.Http;
using System.Threading.Tasks;

using Blazor.Extensions.Logging;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using ICEBG.Client.Infrastructure.ClientServices;
using ICEBG.AppConfig;
using ICEBG.Client;
using Microsoft.AspNetCore.Components.Web;

namespace ICEBG.Blazor;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.Logging.SetMinimumLevel(LogLevel.Information);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // Initialize defaults
        ApplicationConfigurationWASM.Initialize();

        builder.Services.AddScoped(
            sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        builder.Services.AddLogging(builder => builder
            .AddBrowserConsole() // Add Blazor.Extensions.Logging.BrowserConsoleLogger
            .SetMinimumLevel(LogLevel.Information)
        );

        builder.Services.AddOptions();

        ClientServices.Inject(ApplicationConfiguration.pDataServicesEndpointPrefix, builder.Services);

        await builder.Build().RunAsync();
    }

}

