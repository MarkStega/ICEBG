namespace ICEBG.Web.UserInterface;

/// <summary>
/// A class that determines the current platform as being either Blazor Server or WebAssembly.
/// </summary>
public static class PlatformDetermination
{
#if BLAZOR_SERVER
    /// <summary>
    /// We are running Blazor Server if true.
    /// </summary>
    public const bool kIsBlazorServer = true;


    /// <summary>
    /// We are not running Blazor WebAssembly if true.
    /// </summary>
    public const bool kIsBlazorWebAssembly = false;
#else
    /// <summary>
    /// We are not running Blazor Server if false.
    /// </summary>
    public const bool kIsBlazorServer = false;


    /// <summary>
    /// We are running Blazor WebAssembly if false.
    /// </summary>
    public const bool kIsBlazorWebAssembly = true;
#endif

#if DEVELOP
    /// <summary>
    /// We are in development mode if true.
    /// </summary>
    public const bool kIsDevelopment = true;
#else
    /// <summary>
    /// We are not in development mode if false.
    /// </summary>
    public const bool kIsDevelopment = false;
#endif


}
