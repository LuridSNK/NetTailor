using Microsoft.Extensions.DependencyInjection;

namespace NetTailor;

public interface IHttpServiceBuilder
{
    IHttpClientBuilder Create(string name, Action<HttpClient> configureClient);
    IHttpClientBuilder Create(Action<HttpClient> configureClient);
}

