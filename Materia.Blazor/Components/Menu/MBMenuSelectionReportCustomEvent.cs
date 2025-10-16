using Microsoft.AspNetCore.Components;

namespace Materia.Blazor;

[EventHandler(
    attributeName: "onmenuselectionreport",
    eventArgsType: typeof(MenuSelectionReportEventArgs),
    enableStopPropagation: false,
    enablePreventDefault: false)]
public static class EventHandlers
{
}
