using Johnson.Common.Models.Registry;
using Microsoft.AspNetCore.Http;

namespace Johnson.ServiceAdapter.Client;

public interface IDiscoveryClient
{
    Task<IList<ServiceListResponse>> GetServicesListAsync();
    Task<HttpResponse> SendRequestToServiceAsync(); 
}
