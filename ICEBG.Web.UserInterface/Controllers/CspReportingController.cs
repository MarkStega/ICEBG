using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ICEBG.Web.UserInterface;

/// <summary>
/// An endpoint for CSP reporting.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CspReportingController : Controller
{
    private ILogger pLogger { get; set; }
    private CspReportingController() { }
    public CspReportingController(ILogger<CspReportingController> logger)
    {
        pLogger = logger;
        pLogger.LogInformation("CspReportingController.ctor()");
    }

    [HttpPost("UriReport")]
    [AllowAnonymous]
    public async Task<IActionResult> UriReport([FromForm] string request)
    {
        await Task.CompletedTask;
        pLogger.LogError("CSP violation: " + request);
        return Ok();
    }
}
