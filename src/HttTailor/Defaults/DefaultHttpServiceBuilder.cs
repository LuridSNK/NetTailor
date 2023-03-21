using HttTailor.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace HttTailor.Defaults;

public class DefaultHttpServiceBuilder : IHttpServiceBuilder
{
    private readonly IServiceCollection _services;
    public DefaultHttpServiceBuilder(IServiceCollection services)
    {
        _services = services;
    }
    
    public IHttpClientBuilder Create(string name, Action<HttpClient> configureClient)
    {
        return string.IsNullOrEmpty(name) ? 
            _services.AddHttpClientProfile(configureClient) : 
            _services.AddHttpClientProfile(name, configureClient);
    }
    
    public IHttpClientBuilder Create(Action<HttpClient> configureClient)
    {
        return _services.AddHttpClientProfile(configureClient);
    }
}