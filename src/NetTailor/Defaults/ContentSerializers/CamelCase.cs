using Microsoft.IO;

namespace NetTailor.Defaults.ContentSerializers;

internal class CamelCase : AbstractReaderWriter
{
    public CamelCase(RecyclableMemoryStreamManager memoryStreamManager) : base(memoryStreamManager)
    {
    }
}