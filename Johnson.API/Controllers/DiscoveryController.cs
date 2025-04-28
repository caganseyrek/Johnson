using Johnson.API.Services.Discovery;
using Johnson.API.Services.InternalSecurity;
using Johnson.Common.Models.Registry;
using Microsoft.AspNetCore.Mvc;

namespace Johnson.API.Controllers;

[Route("discovery")]
[ApiController]
public class DiscoveryController : ControllerBase
{
    private readonly IRegistryService _registryService;
    private readonly IAPIKeyValidatorService _apiKeyValidatorService;

    public DiscoveryController(IRegistryService discoveryRegistry, IAPIKeyValidatorService apiKeyValidatorService)
    {
        _registryService = discoveryRegistry;
        _apiKeyValidatorService = apiKeyValidatorService;
    }

    [HttpPost]
    [Route("registry")]
    public async Task<IActionResult> RegisterNewServiceAsync([FromBody] ServiceStartupRequest requestBody)
    {
        var validateRequest = await _apiKeyValidatorService.Validate(HttpContext);
        if (!validateRequest)
            return StatusCode(401);

        var isRegistered = await _registryService.RegisterNewServiceAsync(HttpContext, requestBody);
        if (isRegistered is false)
            return StatusCode(500);

        return StatusCode(201);
    }

    [HttpPost]
    [Route("heartbeat")]
    public async Task<IActionResult> RegisterHeartbeatAsync()
    {
        var validateRequest = await _apiKeyValidatorService.Validate(HttpContext);
        if (!validateRequest)
            return StatusCode(401);

        var isRegistered = await _registryService.RegisterHeartbeatAsync(HttpContext);
        if (isRegistered is false)
            return StatusCode(500);

        return StatusCode(200);
    }

    [HttpPost]
    [Route("get-services")]
    public async Task<IActionResult> GetRegisteredServicesAsync()
    {
        var validateRequest = await _apiKeyValidatorService.Validate(HttpContext);
        if (!validateRequest)
            return StatusCode(401);

        var registeredServices = await _registryService.GetRegisteredServicesAsync(HttpContext);
        return StatusCode(200, registeredServices);
    }
}
