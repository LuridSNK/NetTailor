using System.Net.Http.Headers;

namespace NetTailor.Abstractions;

/// <summary>
/// Provides headers for a given request
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public interface IHeaderProvider<in TRequest>
{
    /// <summary>
    /// Adds values from <see cref="TRequest"/> to <see cref="HttpRequestHeaders"/>
    /// </summary>
    /// <param name="request">A request</param>
    /// <param name="headers">An instance of <see cref="HttpRequestHeaders"/></param>
    /// <param name="ct"><see cref="CancellationToken"/></param>
    public ValueTask Provide(TRequest request, HttpRequestHeaders headers, CancellationToken ct = default);
}