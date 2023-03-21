using System.Text.Json;
using NetTailor.Abstractions;
using NetTailor.Extensions;

namespace NetTailor.Defaults;

public class DefaultRequestNamingGetter<TRequest> : IDefaultRequestNamingGetter<TRequest>
{
    public DefaultRequestNamingGetter(Naming naming)
    {
        JsonOptions = JsonSerializerOptionsCache.GetSettingsOrDefault(naming);
    }
    
    public JsonSerializerOptions JsonOptions { get; }
}