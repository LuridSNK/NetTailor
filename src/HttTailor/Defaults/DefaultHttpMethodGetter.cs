using HttTailor.Abstractions;

namespace HttTailor.Defaults;

internal sealed class DefaultHttpMethodGetter<TRequest> : IHttpMethodGetter<TRequest>
{
    public DefaultHttpMethodGetter(HttpMethod method) => Method = method;
    public HttpMethod Method { get; }
}