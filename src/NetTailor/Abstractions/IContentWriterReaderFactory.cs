namespace NetTailor.Abstractions;

/// <summary>
/// Provides named instances of <see cref="IJsonContentSerializer"/>
/// </summary>
public interface IContentWriterReaderFactory
{
    /// <summary>
    /// Creates an implementation of <see cref="IContentReader"/> by name
    /// </summary>
    /// <param name="name">A key for creation for specific request</param>
    /// <returns>An instance of <see cref="IContentReader"/></returns>
    IContentReader CreateReader(string name);
    
    /// <summary>
    /// Creates an implementation of <see cref="IContentWriter"/> by name
    /// </summary>
    /// <param name="name">A key for creation for specific request</param>
    /// <returns>An instance of <see cref="IContentWriter"/></returns>
    IContentWriter CreateWriter(string name);
}