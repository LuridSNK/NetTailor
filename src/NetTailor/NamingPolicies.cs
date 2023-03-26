using System.Text.Json;
using NetTailor.NamingConventions;

namespace NetTailor;

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
    
    /// <summary>
    /// Gets the <see cref="JsonNamingPolicy"/> for the given <see cref="Naming"/> or defaults to <see cref="CamelCase"/>
    /// </summary>
    public static JsonNamingPolicy GetNamingPolicyOrDefault(Naming? naming = null)
    {
        return naming switch
        {
            Naming.LowerSnakeCase => LowerSnakeCase,
            Naming.UpperSnakeCase => UpperSnakeCase,
            _ => CamelCase
        };
    }
}
