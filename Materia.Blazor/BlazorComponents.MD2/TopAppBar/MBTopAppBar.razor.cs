using Materia.Blazor;
using Materia.Blazor.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using System;
using System.Threading.Tasks;

namespace Materia.Blazor.MD2;

/// <summary>
/// A Material Theme top app bar
/// </summary>
public partial class MBTopAppBar : ComponentFoundationMD2
{
    [Parameter] public string Title { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }
    [Parameter] public MBMenuItem[] MenuItems { get; set; }
    [Parameter] public Action<MenuSelectionReportEventArgs> SelectionReport { get; set; }
    [Parameter] public string ScrollTarget { get; set; }
    [Parameter] public MBTopAppBarTypeMD2 TopAppBarType { get; set; } = MBTopAppBarTypeMD2.Standard;

    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;


    private ElementReference HeaderElem { get; set; }


    // Would like to use <inheritdoc/> however DocFX cannot resolve to references outside Material.Blazor
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        //ConditionalCssClasses
        //    .AddIf("mdc-top-app-bar--fixed", () => (TopAppBarType & MBTopAppBarTypeMD2.Fixed) == MBTopAppBarTypeMD2.Fixed)
        //    .AddIf("mdc-top-app-bar--dense", () => (TopAppBarType & MBTopAppBarTypeMD2.Dense) == MBTopAppBarTypeMD2.Dense)
        //    .AddIf("mdc-top-app-bar--prominent", () => (TopAppBarType & MBTopAppBarTypeMD2.Prominent) == MBTopAppBarTypeMD2.Prominent)
        //    .AddIf("mdc-top-app-bar--short", () => (TopAppBarType & MBTopAppBarTypeMD2.Short) == MBTopAppBarTypeMD2.Short)
        //    .AddIf("mdc-top-app-bar--short mdc-top-app-bar--short-collapsed", () => (TopAppBarType & MBTopAppBarTypeMD2.ShortCollapsed) == MBTopAppBarTypeMD2.ShortCollapsed);
    }


    /// <inheritdoc/>
    internal override Task InstantiateMcwComponent()
    {
        return InvokeJsVoidAsync("MaterialBlazor.MBTopAppBar.init", HeaderElem, ScrollTarget);
    }

}
