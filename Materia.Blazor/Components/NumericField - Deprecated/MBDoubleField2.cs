using Materia.Blazor.Internal;
using System;

namespace Materia.Blazor;

/// <summary>
/// A Materia.Blazor formatted double field.
/// </summary>
public sealed class MBDoubleField2 : InternalFloatingPointFieldBase2<double, MBTextField2>
{
    private protected override double ConvertFromDecimal(decimal decimalValue)
    {
        return Convert.ToDouble(decimalValue);
    }
}
