using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NetTailor.Abstractions;
using NetTailor.Contracts;
using NetTailor.Defaults;
using NetTailor.Defaults.Deserializers;

namespace NetTailor.Extensions;

/// <summary>
/// Extensibility methods for <see cref="IHttpClientFactory"/>
/// </summary>
public static class HttpClientBuilderExtensions
{
    private const string ClientHasNoNameMessage = "A client must have a name";

    /// <summary>
    /// Adds a HEAD request to named <see cref="HttpClient"/>
    /// </summary>
    /// <param name="builder">A builder for configuring named <see cref="HttpClient"/> instances returned by <see cref="IHttpClientFactory"/>.</param>
    /// <param name="configureRoute">A delegate that is used to configure <see cref="IEndpointBuilder{TRequest}"/></param>
    /// <param name="configureRequest">A delegate that is used to configure <see cref="HttpRequestMessage"/></param>
    /// <typeparam name="TRequest">A request message object that triggers the Http call</typeparam>
    /// <typeparam name="TResponse">A response that the client returns from Http call</typeparam>
    public static IHttpClientBuilder Head<TRequest, TResponse>(
        this IHttpClientBuilder builder, 
        Expression<Func<TRequest, string>> configureRoute,
        Action<IHttpRequestBuilder<TRequest>> configureRequest)
        where TRequest : IHttpRequest<TResponse>
    {
        CheckArgumentsAndThrowIfAnyIsNull(builder, configureRoute, configureRequest);
        
        builder.Services.ConfigureMethodAndEndpointFor<TRequest, TResponse>(builder.Name, HttpMethod.Head, configureRoute);

        var requestBuilder = new DefaultHttpRequestBuilder<TRequest>(builder.Services);
        configureRequest.Invoke(requestBuilder);
        return builder;
    }

    /// <summary>
    /// Adds a HEAD request to named <see cref="HttpClient"/>
    /// </summary>
    /// <param name="builder">A builder for configuring named <see cref="HttpClient"/> instances returned by <see cref="IHttpClientFactory"/>.</param>
    /// <param name="route">A string that is used to configure <see cref="IEndpointBuilder{TRequest}"/></param>
    /// <param name="configureRequest">A delegate that is used to configure <see cref="HttpRequestMessage"/></param>
    /// <typeparam name="TRequest">A request message object that triggers the Http call</typeparam>
    /// <typeparam name="TResponse">A response that the client returns from Http call</typeparam>
    public static IHttpClientBuilder Head<TRequest, TResponse>(
        this IHttpClientBuilder builder,
        string route,
        Action<IHttpRequestBuilder<TRequest>> configureRequest)
        where TRequest : IHttpRequest<TResponse> 
        => builder.Head<TRequest, TResponse>(_ => route, configureRequest);
    
    /// <summary>
    /// Adds a GET request to named <see cref="HttpClient"/>
    /// </summary>
    /// <param name="builder">A builder for configuring named <see cref="HttpClient"/> instances returned by <see cref="IHttpClientFactory"/>.</param>
    /// <param name="configureRoute">A delegate that is used to configure <see cref="IEndpointBuilder{TRequest}"/></param>
    /// <param name="configureRequest">A delegate that is used to configure <see cref="HttpRequestMessage"/></param>
    /// <typeparam name="TRequest">A request message object that triggers the Http call</typeparam>
    /// <typeparam name="TResponse">A response that the client returns from Http call</typeparam>
    public static IHttpClientBuilder Get<TRequest, TResponse>(
        this IHttpClientBuilder builder, 
        Expression<Func<TRequest, string>> configureRoute,
        Action<IHttpRequestBuilder<TRequest>> configureRequest)
        where TRequest : IHttpRequest<TResponse>
    {
        CheckArgumentsAndThrowIfAnyIsNull(builder, configureRoute, configureRequest);
        
        builder.Services.ConfigureMethodAndEndpointFor<TRequest, TResponse>(builder.Name, HttpMethod.Get, configureRoute);
        
        var requestBuilder = new DefaultHttpRequestBuilder<TRequest>(builder.Services);
        configureRequest.Invoke(requestBuilder);
        return builder;
    }
    
    /// <summary>
    /// Adds a GET request to named <see cref="HttpClient"/>
    /// </summary>
    /// <param name="builder">A builder for configuring named <see cref="HttpClient"/> instances returned by <see cref="IHttpClientFactory"/>.</param>
    /// <param name="route">A string that is used to configure <see cref="IEndpointBuilder{TRequest}"/></param>
    /// <param name="configureRequest">A delegate that is used to configure <see cref="HttpRequestMessage"/></param>
    /// <typeparam name="TRequest">A request message object that triggers the Http call</typeparam>
    /// <typeparam name="TResponse">A response that the client returns from Http call</typeparam>
    public static IHttpClientBuilder Get<TRequest, TResponse>(
        this IHttpClientBuilder builder,
        string route,
        Action<IHttpRequestBuilder<TRequest>> configureRequest)
        where TRequest : IHttpRequest<TResponse> 
        => builder.Get<TRequest, TResponse>(_ => route, configureRequest);

    /// <summary>
    /// Adds a POST request to named <see cref="HttpClient"/>
    /// </summary>
    /// <param name="builder">A builder for configuring named <see cref="HttpClient"/> instances returned by <see cref="IHttpClientFactory"/>.</param>
    /// <param name="configureRoute">A delegate that is used to configure <see cref="IEndpointBuilder{TRequest}"/></param>
    /// <param name="configureRequest">A delegate that is used to configure <see cref="HttpRequestMessage"/></param>
    /// <typeparam name="TRequest">A request message object that triggers the Http call</typeparam>
    /// <typeparam name="TResponse">A response that the client returns from Http call</typeparam>
    public static IHttpClientBuilder Post<TRequest, TResponse>(
        this IHttpClientBuilder builder,
        Expression<Func<TRequest, string>> configureRoute,
        Action<IHttpRequestBuilder<TRequest>> configureRequest)
        where TRequest : IHttpRequest<TResponse>
    {
        CheckArgumentsAndThrowIfAnyIsNull(builder, configureRoute, configureRequest);
        
        builder.Services.ConfigureMethodAndEndpointFor<TRequest, TResponse>(builder.Name, HttpMethod.Post, configureRoute);
        
        var requestBuilder = new DefaultHttpRequestBuilder<TRequest>(builder.Services);
        configureRequest.Invoke(requestBuilder);
        return builder;
    }

    /// <summary>
    /// Adds a POST request to named <see cref="HttpClient"/>
    /// </summary>
    /// <param name="builder">A builder for configuring named <see cref="HttpClient"/> instances returned by <see cref="IHttpClientFactory"/>.</param>
    /// <param name="route">A string that is used to configure <see cref="IEndpointBuilder{TRequest}"/></param>
    /// <param name="configureRequest">A delegate that is used to configure <see cref="HttpRequestMessage"/></param>
    /// <typeparam name="TRequest">A request message object that triggers the Http call</typeparam>
    /// <typeparam name="TResponse">A response that the client returns from Http call</typeparam>
    public static IHttpClientBuilder Post<TRequest, TResponse>(
        this IHttpClientBuilder builder,
        string route,
        Action<IHttpRequestBuilder<TRequest>> configureRequest)
        where TRequest : IHttpRequest<TResponse>
        => builder.Post<TRequest, TResponse>(_ => route, configureRequest);
    
    
    /// <summary>
    /// Adds a PUT request to named <see cref="HttpClient"/>
    /// </summary>
    /// <param name="builder">A builder for configuring named <see cref="HttpClient"/> instances returned by <see cref="IHttpClientFactory"/>.</param>
    /// <param name="configureRoute">A delegate that is used to configure <see cref="IEndpointBuilder{TRequest}"/></param>
    /// <param name="configureRequest">A delegate that is used to configure <see cref="HttpRequestMessage"/></param>
    /// <typeparam name="TRequest">A request message object that triggers the Http call</typeparam>
    /// <typeparam name="TResponse">A response that the client returns from Http call</typeparam>
    public static IHttpClientBuilder Put<TRequest, TResponse>(
        this IHttpClientBuilder builder,
        Expression<Func<TRequest, string>> configureRoute,
        Action<IHttpRequestBuilder<TRequest>> configureRequest)
        where TRequest : IHttpRequest<TResponse>
    {
        CheckArgumentsAndThrowIfAnyIsNull(builder, configureRoute, configureRequest);
        
        builder.Services.ConfigureMethodAndEndpointFor<TRequest, TResponse>(builder.Name, HttpMethod.Put, configureRoute);
        
        var requestBuilder = new DefaultHttpRequestBuilder<TRequest>(builder.Services);
        configureRequest.Invoke(requestBuilder);
        return builder;
    }

    /// <summary>
    /// Adds a PUT request to named <see cref="HttpClient"/>
    /// </summary>
    /// <param name="builder">A builder for configuring named <see cref="HttpClient"/> instances returned by <see cref="IHttpClientFactory"/>.</param>
    /// <param name="route">A string that is used to configure <see cref="IEndpointBuilder{TRequest}"/></param>
    /// <param name="configureRequest">A delegate that is used to configure <see cref="HttpRequestMessage"/></param>
    /// <typeparam name="TRequest">A request message object that triggers the Http call</typeparam>
    /// <typeparam name="TResponse">A response that the client returns from Http call</typeparam>
    public static IHttpClientBuilder Put<TRequest, TResponse>(
        this IHttpClientBuilder builder,
        string route,
        Action<IHttpRequestBuilder<TRequest>> configureRequest)
        where TRequest : IHttpRequest<TResponse>
        => builder.Put<TRequest, TResponse>(_ => route, configureRequest);
    
    
    /// <summary>
    /// Adds a PATCH request to named <see cref="HttpClient"/>
    /// </summary>
    /// <param name="builder">A builder for configuring named <see cref="HttpClient"/> instances returned by <see cref="IHttpClientFactory"/>.</param>
    /// <param name="configureRoute">A delegate that is used to configure <see cref="IEndpointBuilder{TRequest}"/></param>
    /// <param name="configureRequest">A delegate that is used to configure <see cref="HttpRequestMessage"/></param>
    /// <typeparam name="TRequest">A request message object that triggers the Http call</typeparam>
    /// <typeparam name="TResponse">A response that the client returns from Http call</typeparam>
    public static IHttpClientBuilder Patch<TRequest, TResponse>(
        this IHttpClientBuilder builder,
        Expression<Func<TRequest, string>> configureRoute,
        Action<IHttpRequestBuilder<TRequest>> configureRequest)
        where TRequest : IHttpRequest<TResponse>
    {
        CheckArgumentsAndThrowIfAnyIsNull(builder, configureRoute, configureRequest);
        
        builder.Services.ConfigureMethodAndEndpointFor<TRequest, TResponse>(builder.Name, new HttpMethod("PATCH"), configureRoute);
        
        var requestBuilder = new DefaultHttpRequestBuilder<TRequest>(builder.Services);
        configureRequest.Invoke(requestBuilder);
        return builder;
    }

    /// <summary>
    /// Adds a PATCH request to named <see cref="HttpClient"/>
    /// </summary>
    /// <param name="builder">A builder for configuring named <see cref="HttpClient"/> instances returned by <see cref="IHttpClientFactory"/>.</param>
    /// <param name="route">A string that is used to configure <see cref="IEndpointBuilder{TRequest}"/></param>
    /// <param name="configureRequest">A delegate that is used to configure <see cref="HttpRequestMessage"/></param>
    /// <typeparam name="TRequest">A request message object that triggers the Http call</typeparam>
    /// <typeparam name="TResponse">A response that the client returns from Http call</typeparam>
    public static IHttpClientBuilder Patch<TRequest, TResponse>(
        this IHttpClientBuilder builder,
        string route,
        Action<IHttpRequestBuilder<TRequest>> configureRequest)
        where TRequest : IHttpRequest<TResponse>
        => builder.Patch<TRequest, TResponse>(_ => route, configureRequest);
    
    
    /// <summary>
    /// Adds a DELETE request to named <see cref="HttpClient"/>
    /// </summary>
    /// <param name="builder">A builder for configuring named <see cref="HttpClient"/> instances returned by <see cref="IHttpClientFactory"/>.</param>
    /// <param name="configureRoute">A delegate that is used to configure <see cref="IEndpointBuilder{TRequest}"/></param>
    /// <param name="configureRequest">A delegate that is used to configure <see cref="HttpRequestMessage"/></param>
    /// <typeparam name="TRequest">A request message object that triggers the Http call</typeparam>
    /// <typeparam name="TResponse">A response that the client returns from Http call</typeparam>
    public static IHttpClientBuilder Delete<TRequest, TResponse>(
        this IHttpClientBuilder builder,
        Expression<Func<TRequest, string>> configureRoute,
        Action<IHttpRequestBuilder<TRequest>> configureRequest)
        where TRequest : IHttpRequest<TResponse>
    {
        CheckArgumentsAndThrowIfAnyIsNull(builder, configureRoute, configureRequest);
        
        builder.Services.ConfigureMethodAndEndpointFor<TRequest, TResponse>(builder.Name, HttpMethod.Delete, configureRoute);
        
        var requestBuilder = new DefaultHttpRequestBuilder<TRequest>(builder.Services);
        configureRequest.Invoke(requestBuilder);
        return builder;
    }

    
    /// <summary>
    /// Adds a DELETE request to named <see cref="HttpClient"/>
    /// </summary>
    /// <param name="builder">A builder for configuring named <see cref="HttpClient"/> instances returned by <see cref="IHttpClientFactory"/>.</param>
    /// <param name="route">A string that is used to configure <see cref="IEndpointBuilder{TRequest}"/></param>
    /// <param name="configureRequest">A delegate that is used to configure <see cref="HttpRequestMessage"/></param>
    /// <typeparam name="TRequest">A request message object that triggers the Http call</typeparam>
    /// <typeparam name="TResponse">A response that the client returns from Http call</typeparam>
    public static IHttpClientBuilder Delete<TRequest, TResponse>(
        this IHttpClientBuilder builder,
        string route,
        Action<IHttpRequestBuilder<TRequest>> configureRequest)
        where TRequest : IHttpRequest<TResponse>
        => builder.Delete<TRequest, TResponse>(_ => route, configureRequest);
    

    private static void ConfigureMethodAndEndpointFor<TRequest, TResponse>(
        this IServiceCollection services, 
        string clientName, 
        HttpMethod method, 
        Expression<Func<TRequest, string>> configureRoute)
    {
        Debug.WriteLine($"Registering [{method}] request for client '{clientName}' with types [{typeof(TRequest)}:{typeof(TResponse)}]");

        services.AddSingleton<IClientNameGetter<TRequest>, DefaultClientNameGetter<TRequest>>(
            _ => new DefaultClientNameGetter<TRequest>(clientName));
        
        services.AddSingleton<IHttpMethodGetter<TRequest>, DefaultHttpMethodGetter<TRequest>>(
            _ => new DefaultHttpMethodGetter<TRequest>(method));
        
        services.AddSingleton<IEndpointBuilder<TRequest>, DefaultEndpointBuilder<TRequest>>(
            _ => new DefaultEndpointBuilder<TRequest>(configureRoute));

        if (Empty.Value is TResponse)
        {
            services.AddSingleton<IResponseDeserializer<TRequest, TResponse>>(_ => 
                (IResponseDeserializer<TRequest, TResponse>)new EmptyResponseDeserializer<TRequest>());
        }
        else
        {
            services.AddSingleton<
                IResponseDeserializer<TRequest, TResponse>, 
                DefaultResponseDeserializer<TRequest, TResponse>>(
                _ => new DefaultResponseDeserializer<TRequest, TResponse>(JsonSerializerOptionsCache.CamelCase));
        }
    }
    
    private static void CheckArgumentsAndThrowIfAnyIsNull<TRequest>(
        IHttpClientBuilder builder, 
        Expression<Func<TRequest, string>> configureRoute, 
        Action<IHttpRequestBuilder<TRequest>> configureRequest)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));
        if (configureRoute == null) throw new ArgumentNullException(nameof(configureRoute));
        if (configureRequest == null) throw new ArgumentNullException(nameof(configureRequest));
        if (string.IsNullOrEmpty(builder.Name)) throw new InvalidOperationException(ClientHasNoNameMessage);
    }
}