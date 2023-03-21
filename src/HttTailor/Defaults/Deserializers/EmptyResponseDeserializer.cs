using System.Text.Json;
using HttTailor.Abstractions;
using HttTailor.Contracts;

namespace HttTailor.Defaults.Deserializers;

public class EmptyResponseDeserializer<TRequest> : IResponseDeserializer<TRequest, Empty>
{
    public async ValueTask<Empty> Deserialize(HttpContent json, JsonSerializerOptions? serializerOptions = default, CancellationToken ct = default)
    {
        return Empty.Value;
    }
}