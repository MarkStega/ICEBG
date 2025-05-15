using Materia.Blazor;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ICEBG.Client;

/// <summary>
/// The standard Blazor MainLayout component.
/// </summary>
public partial class MainLayout : LayoutComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private readonly MBMenuItem[] menuItems = new MBMenuItem[]
    {
            new MBMenuItem {
                Headline="Home",
                HeadlineColor="darkblue",
                LeadingIcon=MBIcon.IconDescriptorConstructor(
                                    name: "home",
                                    color: "darkblue"),
                MenuItemType=MBMenuItemType.Regular },
            new MBMenuItem {
                Headline="About",
                HeadlineColor="darkblue",
                LeadingIcon=MBIcon.IconDescriptorConstructor(
                                    name: "info",
                                    color: "darkblue"),
                MenuItemType=MBMenuItemType.Regular },
    };

    protected void MenuSelectionReportHandler(MenuSelectionReportEventArgs args)
    {
        var destination = args.menuHeadline.ToLower() switch
        {
            "about" => "about",
            _ => "",
        };
        NavigationManager.NavigateTo(destination);
    }
}
