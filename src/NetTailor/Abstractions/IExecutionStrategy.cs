using System.Net.Http.Headers;
using System.Text.Json;

namespace NetTailor.Abstractions;

public interface IExecutionStrategy<in TRequest, TResponse>
{
    public string ClientName { get; }
    public HttpMethod Method { get; }
    
    public JsonSerializerOptions Options { get; }

    public ValueTask<Uri> BuildEndpoint(TRequest request, CancellationToken ct = default);
    public ValueTask BuildHeaders(TRequest request, HttpRequestHeaders headers, CancellationToken ct = default);
    public ValueTask<HttpContent?> BuildHttpContent(TRequest request, CancellationToken ct = default);

    public ValueTask<TResponse?> Deserialize(HttpContent response, CancellationToken ct = default);
}