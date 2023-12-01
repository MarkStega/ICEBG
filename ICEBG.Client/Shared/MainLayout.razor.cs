using System.Threading.Tasks;

using GoogleAnalytics.Blazor;

using Material.Blazor;
using Material.Blazor.MenuClose;

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


    private readonly MBMenuItem[] menuItems = new MBMenuItem[]
    {
            new MBMenuItem {
                Headline="One",
                HeadlineColor="darkblue",
                MenuItemType=MBMenuItemType.Regular },
            new MBMenuItem {
                Headline="Two",
                HeadlineColor="darkblue",
                MenuItemType=MBMenuItemType.Regular },
            new MBMenuItem {
                Headline="Three",
                HeadlineColor="darkblue",
                MenuItemType=MBMenuItemType.Regular },
            new MBMenuItem
            {
                MenuItemType=MBMenuItemType.Divider
            },
            new MBMenuItem {
                Headline="Four",
                HeadlineColor="darkblue",
                MenuItemType=MBMenuItemType.Regular,
                LeadingIcon=MBIcon.IconDescriptorConstructor(
                                    name: "home",
                                    color: "darkblue")},
            new MBMenuItem {
                Headline="Five",
                HeadlineColor="darkblue",
                MenuItemType=MBMenuItemType.Regular,
                TrailingIcon=MBIcon.IconDescriptorConstructor(
                                    name: "alarm",
                                    color: "darkblue")},
            new MBMenuItem {
                Headline="Six",
                HeadlineColor="darkblue",
                MenuItemType=MBMenuItemType.Regular,
                LeadingIcon=MBIcon.IconDescriptorConstructor(
                                    name: "home",
                                    color: "darkblue"),
                TrailingIcon=MBIcon.IconDescriptorConstructor(
                                    name: "alarm",
                                    color: "darkblue")},
            new MBMenuItem
            {
                MenuItemType=MBMenuItemType.Divider
            },
            new MBMenuItem {
                Headline="Seven",
                HeadlineColor="darkblue",
                MenuItemType=MBMenuItemType.Regular,
                LeadingIcon=MBIcon.IconDescriptorConstructor(
                                    name: "done",
                                    color: "darkgreen"),
                SuppressLeadingIcon=true },
            new MBMenuItem {
                Headline="Eight",
                HeadlineColor="darkblue",
                MenuItemType=MBMenuItemType.Regular,
                LeadingIcon=MBIcon.IconDescriptorConstructor(
                                    name: "done",
                                    color: "darkgreen"),
                SuppressLeadingIcon=true },
            new MBMenuItem
            {
                MenuItemType=MBMenuItemType.Divider
            },
            new MBMenuItem {
                Headline="Nine (disabled)",
                HeadlineColor="darkblue",
                IsDisabled=true,
                MenuItemType=MBMenuItemType.Regular },
    };


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("ICEBG.General.instantiateErrorDialog");
        }
    }

    protected void MW3MenuClose(MenuCloseEventArgs args)
    {

    }
}
