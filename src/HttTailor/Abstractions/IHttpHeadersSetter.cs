using System.Net.Http.Headers;

namespace HttTailor.Abstractions;

public interface IHttpHeadersSetter<in TRequest>
{
    public ValueTask SetHeaders(TRequest request, HttpRequestHeaders headers);
}