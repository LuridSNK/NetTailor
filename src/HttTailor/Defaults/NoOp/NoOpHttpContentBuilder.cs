using System.Text.Json;
using HttTailor.Abstractions;

namespace HttTailor.Defaults.NoOp;

internal sealed class NoOpHttpContentBuilder<TRequest> : IHttpContentBuilder<TRequest>
{
    internal static IHttpContentBuilder<TRequest> Instance { get; } = new NoOpHttpContentBuilder<TRequest>();
    
    public JsonSerializerOptions? Options { get; }
    public async ValueTask<HttpContent?> Build(TRequest request, JsonSerializerOptions? opts = default, CancellationToken ct = default)
    {
        return null;
    }
}