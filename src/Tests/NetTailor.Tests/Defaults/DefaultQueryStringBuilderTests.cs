using System.Text;
using FluentAssertions;
using Microsoft.Extensions.ObjectPool;
using NetTailor.Defaults;

namespace NetTailor.Tests.Defaults;

public class DefaultQueryStringBuilderTests
{
    private readonly ObjectPool<StringBuilder> _stringBuilderObjectPool =
        new DefaultObjectPoolProvider().CreateStringBuilderPool();
    
    [Theory]
    [InlineData(1, "Mercury", false, "?id=1&name=Mercury&isThereLifeThere=False", Naming.CamelCase)]
    [InlineData(2, "Venus", false, "?id=2&name=Venus&is_there_life_there=False", Naming.LowerSnakeCase)]
    [InlineData(3, "Earth", true, "?ID=3&NAME=Earth&IS_THERE_LIFE_THERE=True", Naming.UpperSnakeCase)]
    [InlineData(4, "Mars", null, "?id=4&name=Mars", Naming.CamelCase)]
    [InlineData(5, "Jupiter", false, "?id=5&name=Jupiter&isThereLifeThere=False", Naming.CamelCase)]
    [InlineData(6, "Saturn", false, "?id=6&name=Saturn&isThereLifeThere=False", Naming.CamelCase)]
    [InlineData(7, "Uranus", false, "?ID=7&NAME=Uranus&IS_THERE_LIFE_THERE=False", Naming.UpperSnakeCase)]
    [InlineData(8, "Neptune", false, "?id=8&name=Neptune&isThereLifeThere=False", Naming.CamelCase)]
    [InlineData(null, "Pluto", false, "?name=Pluto&is_there_life_there=False", Naming.LowerSnakeCase)]
    public async Task QueryBuilder_Should_ReturnEmptyQueryString_WhenRequestHasNoProperties(
        int? numberFromSun, 
        string? name, 
        bool? isThereLifeThere, 
        string expected,
        Naming naming)
    {
        var planet = new Planet(numberFromSun, name, isThereLifeThere);
        var builder = new DefaultQueryStringBuilder<Planet>(
            _stringBuilderObjectPool,
            r => new { r.Id, r.Name, r.IsThereLifeThere },
            NamingPolicies.GetNamingPolicyOrDefault(naming)
        );

        var result = await builder.Build(planet);

        result.Should().Be(expected);
    }
   private record Planet(int? Id, string? Name, bool? IsThereLifeThere);
}