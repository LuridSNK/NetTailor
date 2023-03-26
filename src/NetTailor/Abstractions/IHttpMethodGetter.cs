namespace NetTailor.Abstractions;

/// <summary>
/// Gets the HTTP method of a request.
/// </summary>
public interface IHttpMethodGetter<in TRequest>
{
    /// <summary>
    /// The HTTP method of a request.
    /// </summary>
    HttpMethod Method { get; }
}