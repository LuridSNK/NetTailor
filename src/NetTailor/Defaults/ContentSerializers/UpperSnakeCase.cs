using System.Text.Json;
using Microsoft.IO;
using NetTailor.Extensions;

namespace NetTailor.Defaults.ContentSerializers;

public class UpperSnakeCase : AbstractReaderWriter
{
    public UpperSnakeCase(RecyclableMemoryStreamManager memoryStreamManager) : base(memoryStreamManager)
    {
    }

    protected override JsonSerializerOptions Options => JsonSerializerOptionsCache.UpperSnakeCase;
}