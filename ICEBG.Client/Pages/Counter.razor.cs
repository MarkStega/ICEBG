using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

namespace ICEBG.Client.Pages
{
//    [Sitemap(SitemapAttribute.eChangeFreqType.Weekly, 0.8)]
    public partial class Counter : ComponentBase
    {
        private int currentCount = 0;

        private void IncrementCount()
        {
            currentCount++;
        }
    }
}
