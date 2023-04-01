using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IO;
using NetTailor.Defaults.ContentSerializers;

namespace NetTailor.Tests.Defaults.ContentSerializers;

public class LowerSnakeCaseTests
{
    private static readonly RecyclableMemoryStreamManager MemoryStreamManager = new();
    private static LowerSnakeCase LowerSnakeCaseReadWriter => new(MemoryStreamManager);

    [Theory]
    [InlineData("SomeValue", 1, true, new[] { 1 })]
    [InlineData("AnotherValue", 0, false, new[] { 2 })]
    [InlineData((string)null, null, false, new[] { 3 })]
    [InlineData("___", null, true, new[] { 4 })]
    [InlineData("!!!", int.MaxValue, false, new[] { 5 })]
    [InlineData("", int.MinValue, true, new[] { 6 })]
    [InlineData("null", (int.MaxValue / 2), false, new[] { 7 })]
    public async ValueTask LowerSnakeCase_WhenGivenAValue_Should_WriteValidCamelCaseJsonObject(string strValue, int? intValue, bool boolValue, int [] arrayValue)
    {
        var obj = new
        {
            FirstValue = strValue,
            SecondValue = intValue,
            ThirdValue = boolValue
        };
        var content = await LowerSnakeCaseReadWriter.Write(obj);
        var result = await content.ReadAsStringAsync();

        var expectedJson = 
        $@"{{
            ""first_value"":""{strValue}"",
            ""second_value"":{intValue},
            ""third_value"":{boolValue}}},
            ""array_value"":[{arrayValue[0]}]
        }}";
        
        result.Should().BeEquivalentTo(expectedJson);
    }
    
    
    private record SomeObject(string FirstValue, int? SecondValue, bool ThirdValue, int[] ArrayValue);
    [Theory]
    [InlineData("SomeValue", 1, true, new[] { 1, 2, 3, 4 })]
    [InlineData("AnotherValue", 0, false, new[] { 5,6,7,8 })]
    [InlineData((string)null, null, false, new[] { 9, 10, 11, 12 })]
    [InlineData("___", null, true, new[] { 13, 14, 15, 16 })]
    [InlineData("!!!", int.MaxValue, false, new[] { 17, 18, 19, 20 })]
    [InlineData("", int.MinValue, true, new[] { 21, 22, 23, 24 })]
    [InlineData("null", (int.MaxValue / 2), false, new[] { 25, 26, 27, 28 })]
    public async ValueTask LowerSnakeCase_WhenGivenAContent_Should_ReadValidCamelCaseJsonObject(string strValue, int? intValue, bool boolValue, int [] arrayValue)
    {
        var json = 
            $@"{{
            ""first_value"":""{strValue}"",
            ""second_value"":{intValue},
            ""third_value"":{boolValue}}},
            ""array_value"":[{string.Join(", ", arrayValue)}]
        }}";
        var content = new StringContent(json, Encoding.UTF8);
        var result = await LowerSnakeCaseReadWriter.Read<SomeObject>(content);

        result.Should().NotBeNull();
        result!.FirstValue.Should().Be(strValue);
        result.SecondValue.Should().Be(intValue);
        result.ThirdValue.Should().Be(boolValue);
        result.ArrayValue.Should().BeEquivalentTo(arrayValue);
    }
    
    [Theory]
    [InlineData("SomeValue", 1, true, new[] { 1, 2, 3, 4 })]
    [InlineData("AnotherValue", 0, false, new[] { 5,6,7,8 })]
    [InlineData((string)null, null, false, new[] { 9, 10, 11, 12 })]
    [InlineData("___", null, true, new[] { 13, 14, 15, 16 })]
    [InlineData("!!!", int.MaxValue, false, new[] { 17, 18, 19, 20 })]
    [InlineData("", int.MinValue, true, new[] { 21, 22, 23, 24 })]
    [InlineData("null", (int.MaxValue / 2), false, new[] { 25, 26, 27, 28 })]
    public async ValueTask LowerSnakeCase_WhenGivenAContentWithCamelCase_Should_ReadValidCamelCaseJsonObject(string strValue, int? intValue, bool boolValue, int [] arrayValue)
    {
        var json = 
            $@"{{
            ""firstValue"":""{strValue}"",
            ""secondValue"":{intValue},
            ""thirdValue"":{boolValue}}},
            ""arrayValue"":[{string.Join(", ", arrayValue)}]
        }}";
        var content = new StringContent(json, Encoding.UTF8);
        var result = await LowerSnakeCaseReadWriter.Read<SomeObject>(content);

        result.Should().NotBeNull();
        result!.FirstValue.Should().Be(strValue);
        result.SecondValue.Should().Be(intValue);
        result.ThirdValue.Should().Be(boolValue);
        result.ArrayValue.Should().BeEquivalentTo(arrayValue);
    }
    
    [Theory]
    [InlineData("SomeValue", 1, true, new[] { 1, 2, 3, 4 })]
    [InlineData("AnotherValue", 0, false, new[] { 5,6,7,8 })]
    [InlineData((string)null, null, false, new[] { 9, 10, 11, 12 })]
    [InlineData("___", null, true, new[] { 13, 14, 15, 16 })]
    [InlineData("!!!", int.MaxValue, false, new[] { 17, 18, 19, 20 })]
    [InlineData("", int.MinValue, true, new[] { 21, 22, 23, 24 })]
    [InlineData("null", (int.MaxValue / 2), false, new[] { 25, 26, 27, 28 })]
    public async ValueTask LowerSnakeCase_WhenGivenAContentWithUpperSnake_Should_ReadValidCamelCaseJsonObject(string strValue, int? intValue, bool boolValue, int [] arrayValue)
    {
        var json = 
            $@"{{
            ""FIRST_VALUE"":""{strValue}"",
            ""SECOND_VALUE"":{intValue},
            ""THIRD_VALUE"":{boolValue}}},
            ""ARRAY_VALUE"":[{string.Join(", ", arrayValue)}]
        }}";
        var content = new StringContent(json, Encoding.UTF8);
        var result = await LowerSnakeCaseReadWriter.Read<SomeObject>(content);

        result.Should().NotBeNull();
        result!.FirstValue.Should().Be(strValue);
        result.SecondValue.Should().Be(intValue);
        result.ThirdValue.Should().Be(boolValue);
        result.ArrayValue.Should().BeEquivalentTo(arrayValue);
    }
}