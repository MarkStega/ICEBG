using Materia.Blazor.Internal;

namespace Materia.Blazor;

/// <summary>
/// A Materia.Blazor formatted decimal field.
/// </summary>
public sealed class MBDecimalField2 : InternalFloatingPointFieldBase2<decimal, MBTextField2>
{
    private protected override decimal ConvertFromDecimal(decimal decimalValue)
    {
        return decimalValue;
    }
}
