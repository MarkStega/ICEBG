using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Materia.Blazor.Internal;

/// <summary>
/// The base class for all Materia.Blazor.MD2 components.
/// </summary>
public abstract class ComponentFoundationMD2 : ComponentFoundation
{
    #region InstantiateMcwComponent

    /// <summary>
    /// Components should override this with a function to be called when M.B.MD2 wants to run Material Components Web instantiation via JS Interop - always gets called from <see cref="OnAfterRenderAsync(bool)"/>, which should not be overridden.
    /// </summary>
    internal virtual Task InstantiateMcwComponent()
    {
        return Task.CompletedTask;
    }

    #endregion

}
