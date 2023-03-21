namespace HttTailor.Abstractions;

public interface IExecutionStrategyProvider
{
    public IExecutionStrategy<TRequest, TResponse> Provide<TRequest, TResponse>();
}