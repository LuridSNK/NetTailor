using System.Text.Json;

namespace NetTailor.Extensions;

public static class JsonSerializerOptionsCache
{
    internal static JsonSerializerOptions CamelCase { get; } = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = NamingPolicies.CamelCase,
#if DEBUG
        WriteIndented = true
#endif
    };

    internal static JsonSerializerOptions UpperSnakeCase { get; } = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = NamingPolicies.UpperSnakeCase,
        
#if DEBUG
        WriteIndented = true
#endif
        
    };

    internal static JsonSerializerOptions LowerSnakeCase { get; } = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = NamingPolicies.LowerSnakeCase,
#if DEBUG
        WriteIndented = true
#endif
    };

    public static JsonSerializerOptions GetSettingsOrDefault(Naming? naming)
    {
        return naming switch
        {
            Naming.LowerSnakeCase => LowerSnakeCase,
            Naming.UpperSnakeCase => UpperSnakeCase,
            _ => CamelCase
        };
    }
}