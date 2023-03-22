using System.Text.Json;
using NetTailor.Abstractions;

namespace NetTailor.Defaults.NoOp;

internal sealed class NoOpHttpContentBuilder<TRequest> : IHttpContentBuilder<TRequest>
{
    internal static IHttpContentBuilder<TRequest> Instance { get; } = new NoOpHttpContentBuilder<TRequest>();
    
    public JsonSerializerOptions? Options { get; }
    public ValueTask<HttpContent?> Build(
        TRequest request, 
        JsonSerializerOptions? opts = default, 
        CancellationToken ct = default) 
        => new((HttpContent?)null);
}