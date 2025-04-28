using Microsoft.AspNetCore.Http;
using System.Net;

namespace Johnson.API.Services.Utilities;

public static class HttpContextUtils
{
    public static (IPAddress?, int?, string?) Destructure(HttpContext httpContext)
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress;
        var port = httpContext.Connection.RemotePort;
        var scheme = httpContext.Request.Scheme;
        var host = httpContext.Request.Host;

        if (ipAddress is null || scheme is null || string.IsNullOrEmpty(host.ToString()))
        {
            return (null, null, null);
        }
        var URL = $"{scheme}://{host}";

        return (ipAddress, port, URL);
    }
}
