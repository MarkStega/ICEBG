using System.Reflection;
using System.Runtime.InteropServices;

using Microsoft.AspNetCore.Components;

namespace ICEBG.Client.Pages;

[Sitemap(SitemapAttribute.eChangeFreqType.Weekly, 0.8)]
public partial class About : ComponentBase
{
    //		<Configurations>Azure;Develop_Server;Develop_WebAssembly;Release_WebAssembly;</Configurations>

#if Azure
    private string pBuildMode { get; set; } = "Azure";
#endif
#if Develop_Server
    private string pBuildMode { get; set; } = "Develop_Server";
#endif
#if Develop_WebAssembly
    private string pBuildMode { get; set; } = "Develop_WebAssembly";
#endif
#if Release_WebAssembly
    private string pBuildMode { get; set; } = "Release_WebAssembly";
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
        pVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }

}
