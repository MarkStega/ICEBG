using System;

namespace ICEBG.Web.UserInterface;

/// <summary>
/// Produces a nonce for use with CSP.
/// </summary>
public class NonceService
{
    /// <summary>
    /// The Scoped nonce value.
    /// </summary>
    public readonly string nonceValue = "";

    public NonceService()
    {
        var bytes = new byte[32];

        var rnd = new Random();

        rnd.NextBytes(bytes);

        nonceValue = Convert.ToBase64String(bytes);
    }
}
