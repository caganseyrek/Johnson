using Microsoft.AspNetCore.Http;

namespace Johnson.API.Services.Gateway;

public interface IProxyService
{
    Task<HttpResponseMessage?> ForwardRequestAsync(HttpContext httpContext);
}
