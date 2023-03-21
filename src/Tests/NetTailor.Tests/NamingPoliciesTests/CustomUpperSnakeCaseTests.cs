using FluentAssertions;
using NetTailor;

namespace NetTailor.Tests.NamingPoliciesTests;

public class UpperSnakeCaseNamingPolicyTests
{
    [Theory]
    [InlineData("helloWorld", "HELLO_WORLD")]
    [InlineData("HelloWorld", "HELLO_WORLD")]
    [InlineData("hello_world", "HELLO_WORLD")]
    [InlineData("HELLO_WORLD", "HELLO_WORLD")]
    [InlineData("Hello_World", "HELLO_WORLD")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void UpperSnakeCaseJsonNamingPolicy_ShouldConvertCorrectly(string input, string expectedOutput)
    {
        // Arrange
        var policy = NamingPolicies.UpperSnakeCase;

        // Act
        string result = policy.ConvertName(input);
        
        // Assert
        result.Should().Be(expectedOutput);
    }
}