using Johnson.Common.Models.Registry;
using Microsoft.AspNetCore.Http;

namespace Johnson.API.Services.Discovery;

public interface IRegistryService
{
    Task<bool> RegisterNewServiceAsync(HttpContext httpContext, ServiceStartupRequest requestBody);
    Task<bool> RegisterHeartbeatAsync(HttpContext httpContext);
    Task<List<ServiceListResponse>> GetRegisteredServicesAsync(HttpContext httpContext);
}
