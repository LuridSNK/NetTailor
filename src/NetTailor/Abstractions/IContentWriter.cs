namespace NetTailor.Abstractions;

/// <summary>
/// Writes a typed object into <see cref="HttpContent"/>
/// </summary>
public interface IContentWriter
{
    /// <summary>
    /// Writes a specified object to a <see cref="HttpContent"/>
    /// </summary>
    /// <param name="value">An object to write into a <see cref="HttpContent"/></param>
    /// <param name="ct"><see cref="CancellationToken"/></param>
    /// <typeparam name="TObject">Target type of <see cref="IContentWriter"/></typeparam>
    /// <returns></returns>
    ValueTask<HttpContent?> Write<TObject>(TObject value, CancellationToken ct = default);
}