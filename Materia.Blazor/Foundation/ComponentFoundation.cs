using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Materia.Blazor.Internal;

/// <summary>
/// The base class for all Materia.Blazor.MD2 components.
/// </summary>
public abstract class ComponentFoundation : ComponentBase
{
    #region members

    #region Cascading parameters

    [CascadingParameter] protected MBCascadingDefaults CascadingDefaults { get; set; } = new MBCascadingDefaults();

    #endregion

    #region Parameters

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object> UnmatchedAttributes { get; set; }


    /// <summary>
    /// The HTML id attribute is used to specify a unique id for an HTML element.
    ///
    /// You cannot have more than one element with the same id in an HTML document.
    /// </summary>
#pragma warning disable IDE1006 // Naming Styles
    [Parameter] public string id { get; set; }
#pragma warning restore IDE1006 // Naming Styles


    /// <summary>
    /// Additional CSS classes for the component.
    /// </summary>
#pragma warning disable IDE1006 // Naming Styles
    [Parameter] public string @class { get; set; }
#pragma warning restore IDE1006 // Naming Styles


    /// <summary>
    /// Indicates whether the component is disabled.
    /// </summary>

    [Parameter] public bool Disabled { get; set; } = false;


    /// <summary>
    /// Additional CSS style for the component.
    /// </summary>
#pragma warning disable IDE1006 // Naming Styles
    [Parameter] public string style { get; set; }
#pragma warning restore IDE1006 // Naming Styles

    #endregion

    #region Injected properties

    [Inject] private IJSRuntime JsRuntime { get; set; }
    [Inject] private protected ILogger<ComponentFoundation> Logger { get; set; }

    //    [Inject] private protected IMBTooltipService TooltipService { get; set; }
    [Inject] private protected IMBLoggingService LoggingService { get; set; }

    #endregion

    #region Other members

    #endregion

    #endregion

    # region AttributesToSplat

    /// <summary>
    /// Attributes ready for splatting in components. Guaranteed not null, unlike UnmatchedAttributes.
    /// </summary>
    internal IEnumerable<KeyValuePair<string, object>> AttributesToSplat()
    {
        foreach (var attribute in UnmatchedAttributes ?? new Dictionary<string, object>())
        {
            yield return attribute;
        }
    }

    #endregion

    #region InvokeJsVoidAsync

    /// <summary>
    /// Wraps calls to <see cref="BatchingJSRuntime.InvokeVoidAsync"/> adding reference to the batching wrapper (if found). Only
    /// use for components.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    private protected async Task InvokeJsVoidAsync(string identifier, params object[] args)
    {
        await JsRuntime.InvokeVoidAsync(identifier, args).ConfigureAwait(false);
    }

    #endregion

}
