using System.Text.Json;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IO;
using NetTailor.Extensions;

namespace NetTailor.Defaults.ContentSerializers;

public class UpperSnakeCase : AbstractContentReaderWriter
{
    public UpperSnakeCase(RecyclableMemoryStreamManager memoryStreamManager, IContentTypeProvider contentTypeProvider) 
        : base(memoryStreamManager, contentTypeProvider)
    {
    }

    protected override JsonSerializerOptions Options => JsonSerializerOptionsCache.UpperSnakeCase;
}