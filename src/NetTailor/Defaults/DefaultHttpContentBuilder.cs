using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using FastExpressionCompiler;
using Microsoft.IO;
using NetTailor.Abstractions;
using NetTailor.Extensions;

namespace NetTailor.Defaults;

internal sealed class DefaultHttpContentBuilder<TRequest> : IHttpContentBuilder<TRequest>
{
    private const string DefaultContentType = "application/json";
    
    private readonly Func<TRequest, object> _configureContent;
    private readonly RecyclableMemoryStreamManager _memoryStreamManager;

    public DefaultHttpContentBuilder(
        RecyclableMemoryStreamManager memoryStreamManager, 
        Expression<Func<TRequest, object>> configureQuery, 
        JsonSerializerOptions? serializerOptions)
    {
        _memoryStreamManager = memoryStreamManager;
        Options = serializerOptions;
        _configureContent = configureQuery.CompileFast();
    }

    public JsonSerializerOptions? Options { get; }

    public async ValueTask<HttpContent?> Build(TRequest request, JsonSerializerOptions? opts = default, CancellationToken ct = default)
    {
        var obj = _configureContent.Invoke(request);
        using var stream = _memoryStreamManager.GetStream();
        var serializerOpts = Options ?? opts ?? JsonSerializerOptionsCache.CamelCase;
        await JsonSerializer.SerializeAsync(stream, obj, serializerOpts, ct);
        var str = Encoding.UTF8.GetString(stream.ToArray());
        return new StringContent(str, Encoding.UTF8, DefaultContentType);
    }
}