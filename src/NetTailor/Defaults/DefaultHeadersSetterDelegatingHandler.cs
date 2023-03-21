using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using FastExpressionCompiler;
using NetTailor.Abstractions;
using NetTailor.Contracts;
using NetTailor.Extensions;

namespace NetTailor.Defaults;

internal sealed class DefaultHeadersSetterDelegatingHandler : DelegatingHandler, IDefaultHeaderSetter
{
    private readonly Action<FluentHeaderDictionary> _headersSetter;
    
    public DefaultHeadersSetterDelegatingHandler(Expression<Action<FluentHeaderDictionary>> headersSetter)
    {
        if (headersSetter == null) throw new ArgumentNullException(nameof(headersSetter));
        _headersSetter = headersSetter.CompileFast();
    }

    public async ValueTask SetHeaders(HttpRequestHeaders headers)
    {
        var dict = new FluentHeaderDictionary();
        _headersSetter.Invoke(dict);
       FluentHeaderDictionaryExtensions.WriteHeaders(dict, headers);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await SetHeaders(request.Headers);
        return await base.SendAsync(request, cancellationToken);
    }
}