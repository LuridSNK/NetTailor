using System.Net.Http.Headers;
using NetTailor.Abstractions;

namespace NetTailor.Defaults;

internal sealed class DefaultHeaderProvider<TRequest> : IHeaderProvider<TRequest>
{
    private readonly Action<TRequest, HttpRequestHeaders> _configureHeaders;

    public DefaultHeaderProvider(Action<TRequest, HttpRequestHeaders> configureHeaders)
        => _configureHeaders = configureHeaders;

    public ValueTask Provide(TRequest request, HttpRequestHeaders headers, CancellationToken ct = default)
    {
        _configureHeaders.Invoke(request, headers);
        return new ValueTask();
    }
}