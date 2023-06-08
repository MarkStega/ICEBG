//
// ICEBG.Blazor
//
//  2022-11-09  Mark Stega
//              Created
//

using System;
using System.Collections.Generic;
using System.Net.Http;

using Blazor.Extensions.Logging;

using Blazored.LocalStorage;

using GoogleAnalytics.Blazor;

using ICEBG.AppConfig;
using ICEBG.Blazor;
using ICEBG.Client;
using ICEBG.Client.Infrastructure.ClientServices;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Initialize defaults
ApplicationConfigurationWASM.Initialize();

builder.Services.AddScoped(
    sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<INotification, WebAssemblyNotificationService>();

// Add Blazor.Extensions.Logging.BrowserConsoleLogger
builder.Services.AddLogging(
    builder => builder
        .AddBrowserConsole()
        .SetMinimumLevel(LogLevel.Trace)
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
