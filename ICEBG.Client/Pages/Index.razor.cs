using System.Threading.Tasks;

using Material.Blazor;

using Microsoft.AspNetCore.Components;

namespace ICEBG.Client;

/// <summary>
/// The website's index page
/// </summary>
[Sitemap(SitemapAttribute.eChangeFreqType.Weekly, 0.8)]
public partial class Index : ComponentBase
{
    [CascadingParameter] private MainLayout MainLayout { get; set; } = default!;


    private class ImageData
    {
        public string Uri { get; set; } = "";
        public string Caption { get; set; } = "";
        public string Width { get; set; } = "";
        public string Height { get; set; } = "";
        public bool Preload { get; set; } = true;
        public string Rel => Preload ? "preload" : "prefetch";
    }


    [Inject] private NavigationManager NavigationManager { get; set; } = default!;


    private static readonly ImageData[] SkylineImages = new ImageData[]
    {
        new() { Uri = "_content/ICEBG.Client/images/new-york-640.webp", Caption = "New York skyline", Width = "640px", Height = "420px" },
        new() { Uri = "_content/ICEBG.Client/images/new-york-420.webp", Caption = "New York skyline", Width = "420px", Height = "420px" },
        new() { Uri = "_content/ICEBG.Client/images/new-york-320.webp", Caption = "New York skyline", Width = "320px", Height = "420px" },
    };


    private static readonly ImageData[] ProgrammerImages = new ImageData[]
    {
        new() { Uri = "_content/ICEBG.Client/images/programmer-640.webp", Caption = "Programmer working at a desk", Width = "640px", Height = "420px" },
        new() { Uri = "_content/ICEBG.Client/images/programmer-420.webp", Caption = "Programmer working at a desk", Width = "420px", Height = "420px" },
        new() { Uri = "_content/ICEBG.Client/images/programmer-320.webp", Caption = "Programmer working at a desk", Width = "320px", Height = "420px" },
    };


}

