using System.Text.Json;
using NetTailor.Abstractions;
using NetTailor.Contracts;

namespace NetTailor.Defaults.Deserializers;

public class EmptyResponseDeserializer<TRequest> : IResponseDeserializer<TRequest, Empty>
{
    public async ValueTask<Empty> Deserialize(HttpContent json, JsonSerializerOptions? serializerOptions = default, CancellationToken ct = default)
    {
        return Empty.Value;
    }
}