using System.Linq.Expressions;
using System.Net.Http.Headers;
using FastExpressionCompiler;
using HttTailor.Abstractions;
using HttTailor.Contracts;
using HttTailor.Extensions;

namespace HttTailor.Defaults;

internal sealed class DefaultHttpHeadersSetter<TRequest> : IHttpHeadersSetter<TRequest>
{
    private readonly Action<TRequest, FluentHeaderDictionary> _configureHeaders;

    public DefaultHttpHeadersSetter(Expression<Action<TRequest, FluentHeaderDictionary>> configureHeaders)
        => _configureHeaders = configureHeaders.CompileFast();
    
    public async ValueTask SetHeaders(TRequest request, HttpRequestHeaders headers)
    {
        var dict = new FluentHeaderDictionary();
        _configureHeaders.Invoke(request, dict);
        FluentHeaderDictionaryExtensions.WriteHeaders(dict, headers);
    }
}