namespace NetTailor.Abstractions;

/// <summary>
/// A factory that creates an instance of <see cref="IRequestExecutionContext{TRequest,TResponse}"/>
/// </summary>
public interface IRequestExecutionContextFactory
{
    /// <summary>
    /// Creates an implementation of <see cref="IRequestExecutionContext{TRequest,TResponse}"/>
    /// </summary>
    /// <typeparam name="TRequest">A request of the execution context</typeparam>
    /// <typeparam name="TResponse">A response of the execution context</typeparam>
    /// <returns>An instance of <see cref="IRequestExecutionContext{TRequest,TResponse}"/></returns>
    public IRequestExecutionContext<TRequest, TResponse>? Create<TRequest, TResponse>();
}