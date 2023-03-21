using System.Net.Http.Headers;

namespace NetTailor.Abstractions;

public interface IHttpHeadersSetter<in TRequest>
{
    public ValueTask SetHeaders(TRequest request, HttpRequestHeaders headers);
}