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
using Blazored.LocalStorage;
using GoogleAnalytics.Blazor;
using Material.Blazor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using System.Collections.Generic;

namespace ICEBG.Blazor;
public class Program
{
    public static async Task Main(string[] arguments)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(arguments);
        builder.Logging.SetMinimumLevel(LogLevel.Information);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // Initialize defaults
        ApplicationConfigurationWASM.Initialize();

        builder.Services.AddScoped(
            sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        builder.Services.AddScoped<INotification, WebAssemblyNotificationService>();

        builder.Services.AddLogging(builder => builder
            .AddBrowserConsole() // Add Blazor.Extensions.Logging.BrowserConsoleLogger
            .SetMinimumLevel(LogLevel.Information)
        );

        builder.Services.AddOptions();

        ClientServices.Inject(ApplicationConfiguration.pDataServicesEndpointPrefix, builder.Services);

        builder.Services.Configure<CookiePolicyOptions>(options =>
        {
            // Has Pentest fixes
            options.CheckConsentNeeded = context => true;
            options.HttpOnly = HttpOnlyPolicy.Always;
            options.MinimumSameSitePolicy = SameSiteMode.Strict;
            options.Secure = CookieSecurePolicy.Always;
        });

        builder.Services.AddBlazoredLocalStorage();

        Log.Logger = new LoggerConfiguration()
#if DEBUG
.MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("GoogleAnalytics.Blazor", LogEventLevel.Debug)
            .Enrich.FromLogContext()
            .WriteTo.Async(a => a.BrowserConsole(outputTemplate: "{Timestamp:HH:mm:ss.fff}\t[{Level:u3}]\t{Message}{NewLine}{Exception}"))
            .CreateLogger();

        builder.Logging.AddProvider(new SerilogLoggerProvider());

        builder.Services.AddGBService(options =>
        {
            options.TrackingId = "G-2VZJ2X14RH";
            options.GlobalEventParams = new Dictionary<string, object>()
    {
        { Utilities.EventCategory, Utilities.DialogActions },
        { Utilities.NonInteraction, true },
    };
        });

        await builder.Build().RunAsync();
    }

}

