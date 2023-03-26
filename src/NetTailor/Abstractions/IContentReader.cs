namespace NetTailor.Abstractions;

/// <summary>
/// Reads the <see cref="HttpContent"/> and transforms it into a concrete type
/// </summary>
public interface IContentReader
{
    /// <summary>
    /// Reads the stream of <see cref="HttpContent"/> 
    /// </summary>
    /// <param name="content"><see cref="HttpContent"/> from <see cref="HttpResponseMessage"/></param>
    /// <param name="ct"><see cref="CancellationToken"/></param>
    /// <typeparam name="TObject">The response type that is read from <see cref="HttpContent"/></typeparam>
    /// <returns>An object that is being parsed from <see cref="HttpContent"/></returns>
    Task<TObject?> Read<TObject>(HttpContent? content, CancellationToken ct = default) where TObject : class;
}