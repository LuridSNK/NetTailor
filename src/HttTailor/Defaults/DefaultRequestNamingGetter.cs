using System.Text.Json;
using HttTailor.Abstractions;
using HttTailor.Extensions;

namespace HttTailor.Defaults;

public class DefaultRequestNamingGetter<TRequest> : IDefaultRequestNamingGetter<TRequest>
{
    public DefaultRequestNamingGetter(Naming naming)
    {
        JsonOptions = JsonSerializerOptionsCache.GetSettingsOrDefault(naming);
    }
    
    public JsonSerializerOptions JsonOptions { get; }
}