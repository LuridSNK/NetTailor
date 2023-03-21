using System.Text.Json;
using HttTailor.NamingConventions;

namespace HttTailor;

public static class NamingPolicies
{
    /// <summary>
    /// Gets the naming policy for lower_snake_casing
    /// </summary>
    public static JsonNamingPolicy LowerSnakeCase { get; } = new JsonLowerSnakeCaseNamingPolicy();
    
    /// <summary>
    /// Gets the naming policy for UPPER_SNAKE_CASING
    /// </summary>
    public static JsonNamingPolicy UpperSnakeCase { get; } = new JsonUpperSnakeCaseNamingPolicy();
    
    /// <summary>
    /// Gets the naming policy for camelCasing
    /// </summary>
    public static JsonNamingPolicy CamelCase => JsonNamingPolicy.CamelCase;
    
    
    public static JsonNamingPolicy GetNamingPolicyOrDefault(Naming? naming)
    {
        return naming switch
        {
            Naming.LowerSnakeCase => LowerSnakeCase,
            Naming.UpperSnakeCase => UpperSnakeCase,
            _ => CamelCase
        };
    }
}
