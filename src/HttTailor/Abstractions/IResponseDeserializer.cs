using System.Text.Json;

namespace HttTailor.Abstractions;

public interface IResponseDeserializer<TRequest, TResponse>
{
    public ValueTask<TResponse?> Deserialize(HttpContent json, JsonSerializerOptions? serializerOptions = default, CancellationToken ct = default);
}