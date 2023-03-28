using System.Text.Json;
using FluentAssertions;

namespace NetTailor.Tests;

public class JsonNamingPoliciesTests
{
    [Theory]
    [InlineData(Naming.CamelCase)]
    [InlineData(Naming.LowerSnakeCase)]
    [InlineData(Naming.UpperSnakeCase)]
    [InlineData(null)]
    public void NamingPolicies_ShouldBeCorrect(Naming? naming)
    {
        var result = NamingPolicies.GetNamingPolicyOrDefault(naming);
        var name = result.GetType().Name;
        name.Should().Contain(naming?.ToString() ?? "CamelCase");
    }
    
    [Theory]
    [InlineData("camelCase", "camelCase")]
    [InlineData("PascalCase", "pascalCase")]
    [InlineData("SomeKindOfString", "someKindOfString")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void CamelCaseNaming_ShouldConvert(string input, string expected)
    {
        var result = NamingPolicies.CamelCase.ConvertName(input);
        result.Should().Be(expected);
    }
    
    [Theory]
    [InlineData("camelCase", "camel_case")]
    [InlineData("PascalCase", "pascal_case")]
    [InlineData("SomeKindOfString", "some_kind_of_string")]
    [InlineData("the_string", "the_string")]
    [InlineData("OTHER_STR", "other_str")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void LowerSnakeNaming_ShouldConvert(string input, string expected)
    {
        var result = NamingPolicies.LowerSnakeCase.ConvertName(input);
        result.Should().Be(expected);
    }
    
    [Theory]
    [InlineData("camelCase", "CAMEL_CASE")]
    [InlineData("PascalCase", "PASCAL_CASE")]
    [InlineData("SomeKindOfString", "SOME_KIND_OF_STRING")]
    [InlineData("the_string", "THE_STRING")]
    [InlineData("OTHER_STR", "OTHER_STR")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void UpperSnakeNaming_ShouldConvert(string input, string expected)
    {
        var result = NamingPolicies.UpperSnakeCase.ConvertName(input);
        result.Should().Be(expected);
    }
}