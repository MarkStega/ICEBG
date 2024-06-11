using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Materia.Blazor;

///// <summary>
///// Base component for Filled and Outlined Text Fields.
///// Used by NumericFields
///// </summary>

/// <summary>
/// A Materia.Blazor text field.
/// </summary>
public class MXTextField : ComponentBase
{
    #region members

#nullable enable annotations

    #region cascading parameters

    //[CascadingParameter] private MBDateTimeField DateTimeField { get; set; }

    #endregion

    #region non-cascading parameters

//    [Parameter] public MBDensity? Density { get; set; }

    [Parameter] public string? Label { get; set; }

    //[Parameter] public MBIconDescriptor? LeadingIcon { get; set; }

    //[Parameter] public MBIconDescriptor? LeadingToggleIcon { get; set; }

    [Parameter] public string LeadingToggleIconButtonLink { get; set; }

    [Parameter] public string LeadingToggleIconButtonLinkTarget { get; set; }

    [Parameter] public bool LeadingToggleIconSelected { get; set; }

    [Parameter] public string? Prefix { get; set; }

    [Parameter] public string? style { get; set; }

    [Parameter] public string? Suffix { get; set; }

    [Parameter] public string SupportingText { get; set; } = "";

    [Parameter] public bool SupportingTextPersistent { get; set; } = false;

    //[Parameter] public MBTextAlignStyle? TextAlignStyle { get; set; }

    //[Parameter] public string TextFieldId { get; set; } = "textfield-id-" + Guid.NewGuid().ToString().ToLower();

    [Parameter] public MBTextInputStyle TextInputStyle { get; set; } = MBTextInputStyle.Outlined;

    //[Parameter] public MBIconDescriptor? TrailingIcon { get; set; }

    //[Parameter] public MBIconDescriptor? TrailingToggleIcon { get; set; }

    [Parameter] public string TrailingToggleIconButtonLink { get; set; }

    [Parameter] public string TrailingToggleIconButtonLinkTarget { get; set; }

    [Parameter] public bool TrailingToggleIconSelected { get; set; }

    [Parameter] public Expression<Func<object>> ValidationMessageFor { get; set; }

    /// <summary>
    /// Gets or sets the value of the input. This should be used with two-way binding.
    /// </summary>
    /// <example>
    /// @bind-Value="@model.PropertyName"
    /// </example>
    [Parameter] public string Value { get; set; }


    /// <summary>
    /// Gets or sets a callback that updates the bound value.
    /// </summary>
    [Parameter] public EventCallback<string> ValueChanged { get; set; }


    /// <summary>
    /// Gets or sets an expression that identifies the bound value.
    /// </summary>
    [Parameter] public Expression<Func<string>> ValueExpression { get; set; }




    #endregion

#nullable restore annotations

    #region injected members

    //   [Inject] private IJSRuntime JsRuntime { get; set; }

    #endregion

    #region local members

    //   private MBDensity AppliedDensity => CascadingDefaults.AppliedTextFieldDensity(Density);

    #endregion

    #endregion

    #region BuildRenderTree

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //var attributesToSplat = AttributesToSplat().ToArray();

        var valueChanged = EventCallback.Factory.CreateBinder(this, ValueChanged.InvokeAsync, Value);

        builder.OpenElement(0, "materia-outlined-text-field");
        {
            //builder.AddAttribute(rendSeq++, "class", @class);
            //builder.AddAttribute(rendSeq++, "style", style + Utilities.GetTextAlignStyle(CascadingDefaults.AppliedStyle(TextAlignStyle)));
            //builder.AddAttribute(rendSeq++, "id", TextFieldId);

            if (!string.IsNullOrEmpty(Label))
            {
                builder.AddAttribute(10, "label", Label);
            }

            if (!string.IsNullOrEmpty(style))
            {
                builder.AddAttribute(20, "style", style);
            }

            var filled = (TextInputStyle) switch
            {
                MBTextInputStyle.Outlined => false,
                MBTextInputStyle.Filled => true,
                _ => throw new System.Exception("Unknown TextInputStyle")
            };
            if (filled)
            {
                builder.AddAttribute(30, "filled");
            }
        }
        builder.CloseElement();
    }

    #endregion

}
