namespace NetTailor.Abstractions;

/// <summary>
/// Builds endpoints
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public interface IEndpointBuilder<in TRequest>
{
    /// <summary>
    /// Provides a string, based on a given request
    /// </summary>
    /// <param name="request">An object that is used for getting a route values from</param>
    /// <param name="ct"><see cref="CancellationToken"/></param>
    /// <returns>A string, that represents a relative path to the <see cref="Uri"/></returns>
    ValueTask<string> Build(TRequest request, CancellationToken ct = default);
}