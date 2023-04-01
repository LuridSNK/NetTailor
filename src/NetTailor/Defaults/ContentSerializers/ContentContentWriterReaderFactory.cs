using Microsoft.Extensions.Options;
using NetTailor.Abstractions;

namespace NetTailor.Defaults.ContentSerializers;

internal class ContentContentWriterReaderFactory : IContentWriterReaderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptionsMonitor<ContentReaderWriterOptions> _optionsMonitor;

    public ContentContentWriterReaderFactory(IServiceProvider serviceProvider, IOptionsMonitor<ContentReaderWriterOptions> options)
    {
        _optionsMonitor = options;
        _serviceProvider = serviceProvider;
    }

    public IContentReader CreateReader(string name)
    {
        var options = _optionsMonitor.Get(name);
        var reader = options.ContentReader.Invoke(_serviceProvider);
        return reader;
    }
    
    public IContentWriter CreateWriter(string name)
    {
        var options = _optionsMonitor.Get(name);
        var writer = options.ContentWriter.Invoke(_serviceProvider);
        return writer;
    }
}