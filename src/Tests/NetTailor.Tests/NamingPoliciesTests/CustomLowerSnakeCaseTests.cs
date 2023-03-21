using FluentAssertions;
using NetTailor;

namespace NetTailor.Tests.NamingPoliciesTests;

public class LowerSnakeCaseNamingPolicyTests
{
    private const string Word = "hello_world";
    [Theory]
    [InlineData("helloWorld", Word)]
    [InlineData("HelloWorld", Word)]
    [InlineData("hello_world", Word)]
    [InlineData("HELLO_WORLD", Word)]
    [InlineData("Hello_World", Word)]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void LowerSnakeCaseJsonNamingPolicy_ShouldConvertCorrectly(string input, string expectedOutput)
    {
        // Arrange
        var policy = NamingPolicies.LowerSnakeCase;

        // Act
        string result = policy.ConvertName(input);

        // Assert
        result.Should().Be(expectedOutput);
    }
}