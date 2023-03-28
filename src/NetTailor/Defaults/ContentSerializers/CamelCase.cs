using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IO;

namespace NetTailor.Defaults.ContentSerializers;

internal class CamelCase : AbstractContentReaderWriter
{
    public CamelCase(RecyclableMemoryStreamManager memoryStreamManager, IContentTypeProvider contentTypeProvider) 
        : base(memoryStreamManager, contentTypeProvider)
    {
    }
}