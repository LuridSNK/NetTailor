using Microsoft.IO;
using NetTailor.Abstractions;
using NetTailor.Defaults.ContentSerializers;

namespace NetTailor.Tests;

public class HttpContentReaderTests
{
    private static readonly RecyclableMemoryStreamManager MemoryStreamManager = new();

    private static readonly object Obj = new
    {
        FirstValue = "FirstValue",
        SecondValue = "SecondValue",
        ThirdValue = "ThirdValue",
    };
    
    [Fact]
    public void ContentReader_ShouldSerializer_WithCamelCaseProperly()
    {
        var camelCaseReadWriter = new CamelCase(MemoryStreamManager);
        IContentReader contentReader = camelCaseReadWriter;
    }
}