using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using ICEBG.Client;

using Material.Blazor;

namespace ICEBG.Blazor;


/// <summary>
/// Implements <see cref="INotification"/> for the Blazor WebAssembly project, posting messages to a controller on the server.
/// </summary>
public class WebAssemblyNotificationService : INotification
{
    private readonly HttpClient httpClient;
    private readonly IMBToastService mBToastService;


    public WebAssemblyNotificationService(HttpClient httpClient, IMBToastService mBToastService)
    {
        this.httpClient = httpClient;
        this.mBToastService = mBToastService;
    }



    private void NotifyError(HttpResponseMessage response)
    {
        mBToastService.ShowToast(MBToastLevel.Error, "CSP violation: " + response.RequestMessage);
    }
}
