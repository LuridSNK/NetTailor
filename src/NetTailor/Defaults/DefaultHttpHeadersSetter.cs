using System.Net.Http.Headers;
using NetTailor.Abstractions;

namespace NetTailor.Defaults;

internal sealed class DefaultHttpHeadersSetter<TRequest> : IHttpHeadersSetter<TRequest>
{
    private readonly Action<TRequest, HttpRequestHeaders> _configureHeaders;

    public DefaultHttpHeadersSetter(Action<TRequest, HttpRequestHeaders> configureHeaders)
        => _configureHeaders = configureHeaders;
    
    public ValueTask SetHeaders(TRequest request, HttpRequestHeaders headers)
    {
        _configureHeaders.Invoke(request, headers);
        return new ValueTask();
    }
}