using ICEBG.DataTier.DataDefinitions;
using ICEBG.DataTier.gRPCClient;
using ICEBG.DataTier.HelperClasses;
using ICEBG.DataTier.Interfaces;

using Microsoft.AspNetCore.Components;

using System;
using System.Threading.Tasks;
using System.Timers;

namespace ICEBG.Client;

/// <summary>
/// The website's index page
/// </summary>
[Sitemap(SitemapAttribute.eChangeFreqType.Weekly, 0.8)]
public partial class Index : ComponentBase
{ 
}
