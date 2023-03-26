namespace NetTailor.Abstractions;

/// <summary>
/// Query string builder interface that builds a query string from a request object.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public interface IQueryStringBuilder<in TRequest>
{
    /// <summary>
    /// Builds a query string from a request object.
    /// </summary>
    /// <param name="request"><see cref="TRequest"/></param>
    /// <param name="ct"><see cref="CancellationToken"/></param>
    /// <returns>A string value that represents a query string</returns>
    public ValueTask<string> Build(TRequest request, CancellationToken ct = default);
}