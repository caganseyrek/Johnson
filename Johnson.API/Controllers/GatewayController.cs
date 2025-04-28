using Johnson.API.Services.Gateway;
using Microsoft.AspNetCore.Mvc;

namespace Johnson.API.Controllers;

[Route("gateway")]
[ApiController]
public class GatewayController : ControllerBase
{
    private readonly ProxyService _proxyService;

    public GatewayController(ProxyService proxyService)
    {
        _proxyService = proxyService;
    }

    [Route("")]
    [AcceptVerbs("GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS", "HEAD")]
    public async Task<IActionResult> ForwardRequestToServiceAsync()
    {
        var response = await _proxyService.ForwardRequestAsync(HttpContext);
        if (response is null)
            return StatusCode(500);

        return Ok(response);
    }

    [Route("internal")]
    [AcceptVerbs("GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS", "HEAD")]
    public async Task ForwardRequestToInternalServiceAsync()
    {
    }
}
