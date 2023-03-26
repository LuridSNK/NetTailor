using Microsoft.Extensions.DependencyInjection;

namespace NetTailor;

/// <summary>
/// Defines the http service builder
/// </summary>
public interface IHttpServiceBuilder
{
    /// <summary>
    /// Adds a http client profile to the service collection with a given name
    /// </summary>
    /// <param name="name">The name of the profile</param>
    /// <param name="configureClient">A delegate to configure a <see cref="HttpClient"/></param>
    IHttpClientBuilder Create(string name, Action<HttpClient> configureClient);
    
    /// <summary>
    /// Adds a http client profile to the service collection with a randomly generated name
    /// </summary>
    /// <param name="configureClient">A delegate to configure a <see cref="HttpClient"/></param>
    IHttpClientBuilder Create(Action<HttpClient> configureClient);
}

