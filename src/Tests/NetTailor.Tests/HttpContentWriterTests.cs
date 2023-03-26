using FluentAssertions;
using Microsoft.IO;
using NetTailor.Abstractions;
using NetTailor.Defaults.ContentSerializers;

namespace NetTailor.Tests;

public class HttpContentWriterTests
{
    private static readonly RecyclableMemoryStreamManager MemoryStreamManager = new();

    private static readonly object Obj = new
    {
        FirstValue = "FirstValue",
        SecondValue = "SecondValue",
        ThirdValue = "ThirdValue",
    };
        
    [Fact]
    public async ValueTask HttpContentBuilder_WhenGivenNull_ShouldAlsoReturnNull()
    {
        var camelCaseReadWriter = new CamelCase(MemoryStreamManager);
        IContentWriter contentWriter = camelCaseReadWriter;
        var content = await contentWriter.Write((object)null, CancellationToken.None);
        content.Should().BeNull();
    }
    
    [Fact]
    public async ValueTask HttpContentBuilder_WhenGivenAnObject_ShouldReturnJsonWithCamelCaseContainingValues()
    {
        var camelCaseReadWriter = new CamelCase(MemoryStreamManager);
        IContentWriter contentWriter = camelCaseReadWriter;
        var content = await contentWriter.Write(Obj, CancellationToken.None);
        var json = await content!.ReadAsStringAsync();
        json.Should().ContainAll("firstValue", "secondValue", "thirdValue");
    }
    
    [Fact]
    public async ValueTask HttpContentBuilder_WhenGivenAnObject_ShouldReturnJsonWithLowerSnakeCaseContainingValues()
    {
        var camelCaseReadWriter = new LowerSnakeCase(MemoryStreamManager);
        IContentWriter contentWriter = camelCaseReadWriter;
        var content = await contentWriter.Write(Obj, CancellationToken.None);
        var json = await content!.ReadAsStringAsync();
        json.Should().ContainAll("first_value", "second_value", "third_value");
    }
    
    [Fact]
    public async ValueTask HttpContentBuilder_WhenGivenAnObject_ShouldReturnJsonWithUpperSnakeCaseContainingValues()
    {
        var camelCaseReadWriter = new UpperSnakeCase(MemoryStreamManager);
        IContentWriter contentWriter = camelCaseReadWriter;
        var content = await contentWriter.Write(Obj, CancellationToken.None);
        var json = await content!.ReadAsStringAsync();
        json.Should().ContainAll("FIRST_VALUE", "SECOND_VALUE", "THIRD_VALUE");
    }
}