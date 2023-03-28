using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IO;
using NetTailor.Abstractions;
using NetTailor.Extensions;

namespace NetTailor.Defaults.ContentSerializers;

public abstract class AbstractContentReaderWriter : IContentReader, IContentWriter
{
    private readonly RecyclableMemoryStreamManager _memoryStreamManager;
    private readonly IContentTypeProvider _contentTypeProvider;
    
    private readonly MediaTypeHeaderValue _mediaType = new("application/json")
    {
        CharSet = "utf-8"
    };

    protected AbstractContentReaderWriter(RecyclableMemoryStreamManager memoryStreamManager, 
        IContentTypeProvider contentTypeProvider)
    {
        _memoryStreamManager = memoryStreamManager;
        _contentTypeProvider = contentTypeProvider;
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
        if (stream is TObject obj)
        {
            return obj;
        }
        
        return await JsonSerializer.DeserializeAsync<TObject>(stream, Options, ct);
    }

    public async ValueTask<HttpContent?> Write<TObject>(TObject value, CancellationToken ct = default)
    {
        if (value is null) return null;
        if (value is Stream s) return ProcessStream(s);

        var stream = _memoryStreamManager.GetStream();
        await JsonSerializer.SerializeAsync(stream, value, Options, ct);
        var byteArrayContent = new ByteArrayContent(stream.ToArray());
        byteArrayContent.Headers.ContentType = _mediaType;
        return byteArrayContent;
    }

    private HttpContent ProcessStream(Stream stream)
    {
        string? fileName = null;
        var contentType = "application/octet-stream";
        if (stream is FileStream fs)
        {
            fileName = fs.Name;
           contentType = _contentTypeProvider.TryGetContentType(fs.Name, out var type) ? type : contentType;
        }
        
        var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        var form = new MultipartFormDataContent();
        if (fileName is not null)
        {
            form.Add(content, "file", fileName);
        }
        else
        {
            form.Add(content, "file");
        }

        return form;
    }
}