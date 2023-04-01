using Microsoft.IO;

namespace NetTailor.Defaults.ContentSerializers;

internal class CamelCase : AbstractContentReaderWriter
{
    public CamelCase(RecyclableMemoryStreamManager memoryStreamManager) 
        : base(memoryStreamManager)
    {
    }
}