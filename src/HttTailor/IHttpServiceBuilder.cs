using Microsoft.Extensions.DependencyInjection;

namespace HttTailor;

public interface IHttpServiceBuilder
{
    IHttpClientBuilder Create(string name, Action<HttpClient> configureClient);
    IHttpClientBuilder Create(Action<HttpClient> configureClient);
}

