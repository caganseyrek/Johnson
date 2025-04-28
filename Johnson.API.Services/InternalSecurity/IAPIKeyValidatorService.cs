using Microsoft.AspNetCore.Http;

namespace Johnson.API.Services.InternalSecurity;

public interface IAPIKeyValidatorService
{
    Task<bool> Validate(HttpContext httpContext);
}
