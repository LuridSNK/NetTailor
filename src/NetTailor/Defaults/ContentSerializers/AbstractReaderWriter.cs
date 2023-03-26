using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.IO;
using NetTailor.Abstractions;
using NetTailor.Extensions;

namespace NetTailor.Defaults.ContentSerializers;

public abstract class AbstractReaderWriter : IContentReader, IContentWriter
{
    private readonly RecyclableMemoryStreamManager _memoryStreamManager;
    
    private readonly MediaTypeHeaderValue _mediaType = new("application/json")
    {
        CharSet = "utf-8"
    };

    protected AbstractReaderWriter(RecyclableMemoryStreamManager memoryStreamManager)
    {
        _memoryStreamManager = memoryStreamManager;
    }
    
    protected virtual JsonSerializerOptions Options => JsonSerializerOptionsCache.CamelCase;

    public async Task<TObject?> Read<TObject>(HttpContent? content, CancellationToken ct = default) 
        where TObject : class
    {
        if (content is null) return null;
#if NETCOREAPP
        var stream = await content.ReadAsStreamAsync(ct);
#else
        var stream = await content.ReadAsStreamAsync();
#endif
        return await JsonSerializer.DeserializeAsync<TObject>(stream, Options, ct);
    }

    public async ValueTask<HttpContent?> Write<TObject>(TObject value, CancellationToken ct = default)
    {
        if (value is null) return null;
        var stream = _memoryStreamManager.GetStream();
        await JsonSerializer.SerializeAsync(stream, value, Options, ct);
        var byteArrayContent = new ByteArrayContent(stream.ToArray());
        byteArrayContent.Headers.ContentType = _mediaType;
        return byteArrayContent;
    }
}