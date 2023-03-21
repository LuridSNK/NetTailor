using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using HttTailor.Abstractions;

namespace HttTailor.Defaults;

public class DefaultExecutionStrategyFor<TRequest, TResponse> : IExecutionStrategy<TRequest, TResponse>
{
    private readonly IClientNameGetter<TRequest> _clientNameGetter;
    private readonly IHttpMethodGetter<TRequest> _httpMethodGetter;
    private readonly IEndpointBuilder<TRequest> _endpointBuilder;
    private readonly IDefaultRequestNamingGetter<TRequest> _requestNamingGetter;
    private readonly IHttpHeadersSetter<TRequest> _httpHeadersSetter;
    private readonly IQueryStringBuilder<TRequest> _queryStringBuilder;
    private readonly IHttpContentBuilder<TRequest> _httpContentBuilder;
    private readonly IResponseDeserializer<TRequest, TResponse> _responseDeserializer;

    public DefaultExecutionStrategyFor(IClientNameGetter<TRequest> clientNameGetter,
        IHttpMethodGetter<TRequest> httpMethodGetter,
        IEndpointBuilder<TRequest> endpointBuilder,
        IDefaultRequestNamingGetter<TRequest> requestNamingGetter,
        IHttpHeadersSetter<TRequest> httpHeadersSetter,
        IQueryStringBuilder<TRequest> queryStringBuilder,
        IHttpContentBuilder<TRequest> httpContentBuilder,
        IResponseDeserializer<TRequest, TResponse> responseDeserializer)
    {
        _clientNameGetter = clientNameGetter;
        _httpMethodGetter = httpMethodGetter;
        _endpointBuilder = endpointBuilder;
        _requestNamingGetter = requestNamingGetter;
        _httpHeadersSetter = httpHeadersSetter; // can be null
        _queryStringBuilder = queryStringBuilder; // can be null
        _httpContentBuilder = httpContentBuilder; // can be null
        _responseDeserializer = responseDeserializer;
    }

    public string ClientName => _clientNameGetter.Name;
    public HttpMethod Method => _httpMethodGetter.Method;

    public JsonSerializerOptions Options => _requestNamingGetter.JsonOptions;

    public async ValueTask<Uri> BuildEndpoint(TRequest request, CancellationToken ct = default)
    {
        var endpointString = await _endpointBuilder.Build(request);
        var queryString = await _queryStringBuilder.Build(request);
        
        Debug.WriteLine($"[{typeof(TRequest)}:{typeof(TResponse)}] endpoint: {endpointString}");
        Debug.WriteLine($"[{typeof(TRequest)}:{typeof(TResponse)}] query: {queryString}");

        return new Uri(endpointString + queryString, UriKind.Relative);
    }

    public async ValueTask BuildHeaders(TRequest request, HttpRequestHeaders headers, CancellationToken ct = default)
    {
        await _httpHeadersSetter.SetHeaders(request, headers);
    }
    
    public async ValueTask<HttpContent?> BuildHttpContent(TRequest request, CancellationToken ct = default)
    {
        
        if (_httpContentBuilder is null || 
            _httpMethodGetter.Method == HttpMethod.Get ||
            _httpMethodGetter.Method == HttpMethod.Head)
        {
            Debug.WriteLine($"[{typeof(TRequest)}:{typeof(TResponse)}] method {_httpMethodGetter.Method}, but no HttpContent");
            return null;
        }
        
        return await _httpContentBuilder.Build(request, Options, ct);
    }

    public async ValueTask<TResponse?> Deserialize(HttpContent response, CancellationToken ct = default)
    {
        var options = _httpContentBuilder.Options ?? Options;
        return await _responseDeserializer.Deserialize(response, options, ct);
    }
}