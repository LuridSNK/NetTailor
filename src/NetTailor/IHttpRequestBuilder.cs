using System.Linq.Expressions;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using NetTailor.Abstractions;

namespace NetTailor;

/// <summary>
/// An interface for configuring a request
/// </summary>
/// <typeparam name="TRequest">A request</typeparam>
/// <typeparam name="TResponse">Returning value</typeparam>
public interface IHttpRequestBuilder<TRequest, TResponse>
{
    /// <summary>
    /// Gets a collection of services
    /// </summary>
    internal IServiceCollection Services { get; }
    
    /// <summary>
    /// Configures the request body based on a request object.
    /// </summary>
    /// <param name="configureContent">A delegate that is used to configure an object for <see cref="HttpContent"/></param>
    public IHttpRequestBuilder<TRequest, TResponse> Content(Expression<Func<TRequest, object>> configureContent);
    
    /// <summary>
    /// Configures the request query based on a request object.
    /// </summary>
    /// <param name="configureQuery">
    /// A delegate that is used to configure an object for query string.
    /// </param>
    /// <param name="naming">Naming policy for query string</param>
    public IHttpRequestBuilder<TRequest, TResponse> Query(Expression<Func<TRequest, object>> configureQuery, Naming? naming = default);
    
    /// <summary>
    /// Configures the request headers based on a request object.
    /// </summary>
    /// <param name="configureHeaders">
    /// A delegate that is used to configure an object for <see cref="HttpRequestHeaders"/>
    /// </param>
    public IHttpRequestBuilder<TRequest, TResponse> Headers(Action<TRequest, HttpRequestHeaders> configureHeaders);


    /// <summary>
    /// Allows to override the default camelCase reader and use a custom one.
    /// </summary>
    /// <typeparam name="TContentReader"><see cref="IContentReader"/></typeparam>
    public IHttpRequestBuilder<TRequest, TResponse> UseContentReader<TContentReader>()
        where TContentReader : class, IContentReader;

    /// <summary>
    /// Allows to override the default camelCase writer and use a custom one.
    /// </summary>
    /// <typeparam name="TContentWriter"><see cref="IContentWriter"/></typeparam>
    public IHttpRequestBuilder<TRequest, TResponse> UseContentWriter<TContentWriter>()
        where TContentWriter : class, IContentWriter;
}
