using System.Net.Http.Headers;
using HttTailor.Abstractions;

namespace HttTailor.Defaults.NoOp;

internal sealed class NoOpHeaderSetter<TRequest> : IHttpHeadersSetter<TRequest>
{
    internal static IHttpHeadersSetter<TRequest> Instance { get; } = new NoOpHeaderSetter<TRequest>();
    public ValueTask SetHeaders(TRequest request, HttpRequestHeaders headers)
    {
        return new ValueTask();
    }
}