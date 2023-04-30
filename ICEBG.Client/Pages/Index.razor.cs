using Microsoft.AspNetCore.Components;

namespace ICEBG.Client.Pages;

[Sitemap(SitemapAttribute.eChangeFreqType.Weekly, 0.8)]
public partial class Index: ComponentBase
{

    private class ImageData
    {
        public string Uri { get; set; } = "";
        public string Caption { get; set; } = "";
        public string Width { get; set; } = "";
        public string Height { get; set; } = "";
        public bool Preload { get; set; } = false;
        public string Rel => Preload ? "preload" : "prefetch";
    }

    private static readonly ImageData[] CarouselImages = new ImageData[]
    {
        new() { Uri = "_content/ICEBG.Client/images/01-main-screen.webp", Caption = "Dioptra's main screen layout", Preload = true },
        new() { Uri = "_content/ICEBG.Client/images/02-main-screen-search.webp", Caption = "Scheme search" },
        new() { Uri = "_content/ICEBG.Client/images/03-march-costs-chart.webp", Caption = "Budget, actual and forecast development costs" },
        new() { Uri = "_content/ICEBG.Client/images/04-march-accruals.webp", Caption = "Loan interest accrual details" },
        new() { Uri = "_content/ICEBG.Client/images/05-march-cost-summary.webp", Caption = "Project cost summary" },
        new() { Uri = "_content/ICEBG.Client/images/06-march-capital-flows.webp", Caption = "Actual/forecast capital structure" },
        new() { Uri = "_content/ICEBG.Client/images/07-march-land-reg.webp", Caption = "UK Land Registry sold unit prices" },
        new() { Uri = "_content/ICEBG.Client/images/08-march-version-graph.webp", Caption = "Scheme data versioning/audit" },
        new() { Uri = "_content/ICEBG.Client/images/09-march-edit-budget-schedule.webp", Caption = "Editting cost budget schedules" },
    };


}
