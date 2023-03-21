using System.Net.Http.Headers;

namespace HttTailor.Abstractions;

public interface IDefaultHeaderSetter
{
    ValueTask SetHeaders(HttpRequestHeaders headers);
}