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
                Headline="GRPC configuration",
                HeadlineColor="darkblue",
                LeadingIcon=MBIcon.IconDescriptorConstructor(
                                    name: "assignment",
                                    color: "darkblue"),
                MenuItemType=MBMenuItemType.Regular },
            new MBMenuItem {
                Headline="REST weather",
                HeadlineColor="darkblue",
                LeadingIcon=MBIcon.IconDescriptorConstructor(
                                    name: "table_chart",
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
            "grpc configuration" => "configuration",
            "rest weather" => "weather",
            "about" => "about",
            _ => "",
        };
        NavigationManager.NavigateTo(destination);
    }
}
