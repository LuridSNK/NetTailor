namespace NetTailor;


/// <summary>
/// Represents a http service profile implementation
/// </summary>
public interface IHttpServiceProfile
{
    /// <summary>
    /// Configures the http profile
    /// </summary>
    public void Configure(IHttpServiceBuilder builder);
}
