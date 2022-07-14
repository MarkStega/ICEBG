using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace ICEBG.Web.UserInterface.Middleware;
public class NoCacheMiddleware
{
    private readonly RequestDelegate next;
    public NoCacheMiddleware(RequestDelegate next)
    {
        this.next = next;
    }
    public async Task Invoke(HttpContext httpContext)
    {
        httpContext.Response.OnStarting((state) =>
        {
            // ref: <a href="http://stackoverflow.com/questions/49547/making-sure-a-web-page-is-not-cached-across-all-browsers">http://stackoverflow.com/questions/49547/making-sure-a-web-page-is-not-cached-across-all-browsers</a>
            httpContext.Response.Headers.Append("Cache-Control", "public, max-age=86400");
            httpContext.Response.Headers.Append("Pragma", "no-cache");
            httpContext.Response.Headers.Append("Expires", "0");
            return Task.FromResult(0);
        }, null);
        await next.Invoke(httpContext);
    }
}