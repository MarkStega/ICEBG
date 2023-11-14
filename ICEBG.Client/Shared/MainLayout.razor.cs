using System.Threading.Tasks;

using GoogleAnalytics.Blazor;

using Material.Blazor;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ICEBG.Client;

/// <summary>
/// The standard Blazor MainLayout component.
/// </summary>
public partial class MainLayout : LayoutComponentBase
{
    [Inject] private INotification Notifier { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private IGBAnalyticsManager AnalyticsManager { get; set; } = default!;


    private bool HomeButtonExited { get; set; } = true;
    private ContactMessage ContactMessage { get; set; } = new();

    private Material.Blazor.MD2.MBDrawer Drawer { get; set; }


    private void ListItemClickHandler(string NavigationReference)
    {
        Drawer.NotifyNavigation();
        NavigationService.NavigateTo(NavigationReference);
    }

    private void SideBarToggle()
    {
        Drawer.Toggle();
    }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("ICEBG.General.instantiateErrorDialog");
        }
    }
}
