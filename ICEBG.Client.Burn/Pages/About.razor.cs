using System.Reflection;
using System.Runtime.InteropServices;

using Microsoft.AspNetCore.Components;

namespace ICEBG.Client;

[Sitemap(SitemapAttribute.eChangeFreqType.Weekly, 0.8)]
public partial class About : ComponentBase
{

#if AZURE
    private string pBuildMode { get; set; } = "Azure";
#endif
#if DEVSERVER
    private string pBuildMode { get; set; } = "Develop_Server";
#endif
#if DEVWEBASSEMBLY
    private string pBuildMode { get; set; } = "Develop_WebAssembly";
#endif

    private string pOSArchitecture { get; set; }
    private string pOSDescription { get; set; }
    private string pRuntime { get; set; }
    private string pVersion { get; set; }

    public About()
    {
        pOSArchitecture = RuntimeInformation.OSArchitecture.ToString();
        pOSDescription = RuntimeInformation.OSDescription.ToString();
        pRuntime = RuntimeInformation.FrameworkDescription.ToString();
        pVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.Split('+')[0];
    }

}
