using System.Text.Json;

namespace NetTailor.Abstractions;

public interface IHttpContentBuilder<in TRequest>
{
    public JsonSerializerOptions? Options { get; }
    public ValueTask<HttpContent?> Build(TRequest request, JsonSerializerOptions? opts = default, CancellationToken ct = default);
}