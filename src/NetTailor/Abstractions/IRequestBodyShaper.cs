namespace NetTailor.Abstractions;

/// <summary>
/// An object that mutates current Request before it will be written into <see cref="HttpContent"/>
/// </summary>
/// <typeparam name="TRequest">An initial request</typeparam>
public interface IRequestBodyShaper<in TRequest>
{
    /// <summary>
    /// Mutates an object
    /// </summary>
    /// <param name="request">An original request object</param>
    /// <returns>An instance of a mutated object</returns>
    object? Shape(TRequest request);
}