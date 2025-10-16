using Materia.Blazor.Internal;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Materia.Blazor;

/// <summary>
/// Renders icons from the Material Symbol Font. Material Symbols are essential for
/// Materia.Blazor and are included in the library's CSS.
/// </summary>
public class MBIcon : ComponentFoundation
{
    #region members

    /// <summary>
    /// The icon attributes and a constructor for same.
    /// </summary>
    [Parameter] public MBIconDescriptor Descriptor { get; set; }
    public static MBIconDescriptor IconDescriptorConstructor(
        string color = null,
        decimal? fill = null,
        MBIconGradient? gradient = null,
        string name = null,
        MBIconSize? size = null,
        MBIconStyle? style = null,
        MBIconWeight? weight = null)
        =>
        new MBIconDescriptor(
            color: color,
            fill: fill,
            gradient: gradient,
            name: name,
            size: size,
            style: style,
            weight: weight);



    #endregion

    #region BuildRenderTree

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var attributesToSplat = AttributesToSplat().ToArray();
        var rendSeq = 0;

        BuildRenderTreeWorker(
            builder,
            ref rendSeq,
            CascadingDefaults,
            attributesToSplat,
            @class,
            style,
            id,
            Descriptor,
            null);
    }

    public static void BuildRenderTreeWorker(
        RenderTreeBuilder builder,
        ref int rendSeq,
        MBCascadingDefaults cascadingDefaults,
        KeyValuePair<string, object>[] attributesToSplat,
        string classString,
        string styleString,
        string idString,
        MBIconDescriptor descriptor,
        string slot)
    {
        var iconColor = cascadingDefaults.AppliedIconColor(descriptor.Color);
        var iconFill = cascadingDefaults.AppliedIconFill(descriptor.Fill);
        var iconGradient = cascadingDefaults.AppliedIconGradient(descriptor.Gradient);
        var iconName = cascadingDefaults.AppliedIconName(descriptor.Name);
        var iconSize = cascadingDefaults.AppliedIconSize(descriptor.Size);
        var iconSlot = slot;
        var iconStyle = cascadingDefaults.AppliedIconStyle(descriptor.Style);
        var iconWeight = cascadingDefaults.AppliedIconWeight(descriptor.Weight);

        // Set the icon style

        var fontStyle = "";

        // Icon font
        fontStyle += "--md-icon-font: " + iconStyle switch
        {
            MBIconStyle.Outlined => "'Material Symbols Outlined'; ",
            MBIconStyle.Rounded => "'Material Symbols Rounded'; ",
            MBIconStyle.Sharp => "'Material Symbols Sharp'; ",
            _ => throw new System.Exception("Unknown Icon Style")
        };

        // Icon color
        fontStyle += " color: " + iconColor + ";";

        // Icon size
        fontStyle += " --md-icon-size: " + iconSize switch
        {
            MBIconSize.Size20 => "20px; ",
            MBIconSize.Size24 => "24px; ",
            MBIconSize.Size40 => "40px; ",
            MBIconSize.Size48 => "48px; ",
            _ => throw new System.Exception("Unknown Icon Size")
        };

        // Icon weight
        _ = iconWeight switch
        {
            MBIconWeight.W100 => fontStyle += " font-weight: 100;",
            MBIconWeight.W200 => fontStyle += " font-weight: 200;",
            MBIconWeight.W300 => fontStyle += " font-weight: 300;",
            MBIconWeight.W400 => fontStyle += " font-weight: 400;",
            MBIconWeight.W500 => fontStyle += " font-weight: 500;",
            MBIconWeight.W600 => fontStyle += " font-weight: 600;",
            MBIconWeight.W700 => fontStyle += " font-weight: 700;",
            _ => throw new System.Exception("Unknown Icon Weight")
        };

        // Set the icon font variation

        var fontVariation = " font-variation-settings:";

        // Icon fill
        fontVariation += " 'FILL' " + iconFill.ToString() + ",";

        // Icon gradient
        _ = iconGradient switch
        {
            MBIconGradient.LowEmphasis => fontVariation += " 'GRAD' -25,",
            MBIconGradient.NormalEmphasis => fontVariation += " 'GRAD' 0,",
            MBIconGradient.HighEmphasis => fontVariation += " 'GRAD' 200,",
            _ => throw new System.Exception("Unknown Icon Gradient")
        };


        // Fix the trailing part of the variation string
        if (fontVariation.EndsWith(','))
        {
            fontVariation = fontVariation.TrimEnd(',');
            fontVariation += ';';
        }

        if (fontVariation.EndsWith(':'))
        {
            fontVariation = null;
        }

        var iconDerivedStyle = fontStyle + fontVariation + styleString;

        builder.OpenElement(rendSeq, "md-icon");
        {
            if ((attributesToSplat is not null) && attributesToSplat.Any())
            {
                builder.AddMultipleAttributes(rendSeq + 1, attributesToSplat);
            }

            builder.AddAttribute(rendSeq + 2, "class", classString);
            builder.AddAttribute(rendSeq + 3, "style", iconDerivedStyle);
            builder.AddAttribute(rendSeq + 4, "id", idString);

            if (!string.IsNullOrWhiteSpace(iconSlot))
            {
                builder.AddAttribute(rendSeq + 5, "slot", iconSlot);
            }
            builder.AddContent(rendSeq + 6, iconName.ToLower());
        }
        builder.CloseElement();
    }

    #endregion

}
