using System.Text.Json;

namespace NetTailor.Extensions;

public static class JsonSerializerOptionsCache
{
    internal static JsonSerializerOptions CamelCase { get; } = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = NamingPolicies.CamelCase,
        WriteIndented = true
    };

    internal static JsonSerializerOptions UpperSnakeCase { get; } = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = NamingPolicies.UpperSnakeCase,
        WriteIndented = true
    };

    internal static JsonSerializerOptions LowerSnakeCase { get; } = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = NamingPolicies.LowerSnakeCase,
        WriteIndented = true
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