using System.Net.Http.Headers;
using NetTailor.Abstractions;

namespace NetTailor.Defaults.NoOp;

internal sealed class NoOpHeaderProvider<TRequest> : IHeaderProvider<TRequest>
{
    internal static IHeaderProvider<TRequest> Instance { get; } = new NoOpHeaderProvider<TRequest>();
    public ValueTask Provide(TRequest request, HttpRequestHeaders headers, CancellationToken ct) =>  new();
}