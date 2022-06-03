using System;

namespace ICEBG.DataTier.HelperClasses;

public abstract class ValidationUtility
{
    /// <summary>
    /// Validates that the <see cref="System.String"/> value is not <code>null</code> or empty.
    /// </summary>
    /// <param name="name">The name of the argument.</param>
    /// <param name="value">The value of the argument.</param>
    public static void ValidateArgument(string name, string value)
    {
        ValidateArgument(name, value, true);
    }

    /// <summary>
    /// Validates that the <see cref="System.String"/> value is not <code>null</code> or empty.
    /// </summary>
    /// <param name="name">The name of the argument.</param>
    /// <param name="value">The value of the argument.</param>
    /// <param name="checkLength">Indicates if the value's length should be validated.</param>
    public static void ValidateArgument(string name, string value, bool checkLength)
    {
        if (value == null)
        {
            throw new ArgumentNullException(name);
        }

        if (checkLength && value.Length == 0)
        {
            string message = String.Format("{0} cannot be a zero-length string.", name);
            throw new ArgumentException(message);
        }
    }

    /// <summary>
    /// Validates that the <see cref="System.Object"/> value is not <code>null</code>.
    /// </summary>
    /// <param name="name">The name of the argument.</param>
    /// <param name="value">The value of the argument.</param>
    public static void ValidateArgument(string name, object value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(name);
        }
    }
}
