using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Johnson.API;

public class RoutePrefixConvention : IApplicationModelConvention
{
    private readonly string _apiPrefix;
    private readonly string _apiVersion;

    public RoutePrefixConvention(string apiPrefix, string apiVersion)
    {
        _apiPrefix = apiPrefix;
        _apiVersion = apiVersion;
    }

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                        new AttributeRouteModel(new RouteAttribute($"{_apiPrefix}/{_apiVersion}")),
                        selector.AttributeRouteModel);
                }
            }
        }
    }
}
