using System.Diagnostics;
using System.Text.Json;
using NetTailor.Abstractions;
using NetTailor.Extensions;

namespace NetTailor.Defaults.Deserializers;

public class DefaultResponseDeserializer<TRequest, TResponse> : IResponseDeserializer<TRequest, TResponse>
{
    private readonly JsonSerializerOptions _serializerOptions;

    public DefaultResponseDeserializer(
        JsonSerializerOptions serializerOptions)
    {
        _serializerOptions = serializerOptions;
    }

    public async ValueTask<TResponse?> Deserialize(
        HttpContent json, 
        JsonSerializerOptions? serializerOptions = default, 
        CancellationToken ct = default)
    {
        var stream = await json.ReadAsStreamAsync();
        var options = serializerOptions ?? _serializerOptions ?? JsonSerializerOptionsCache.CamelCase;
        var serialized = await JsonSerializer.DeserializeAsync<TResponse>(stream, options, ct);
        Debug.WriteLine(serialized);
        return serialized;
    }
}