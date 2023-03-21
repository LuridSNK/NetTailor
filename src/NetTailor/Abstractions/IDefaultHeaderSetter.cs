using System.Net.Http.Headers;

namespace NetTailor.Abstractions;

public interface IDefaultHeaderSetter
{
    ValueTask SetHeaders(HttpRequestHeaders headers);
}