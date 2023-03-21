#nullable enable

namespace NetTailor.Contracts;

public record struct HttpResult<TResult>
(
    TResult? Value, 
    bool Successful, 
    Exception? Exception)
{
    public TResult? Value { get; } = Value;
    public bool Successful { get; } = Successful;
    public Exception? Exception { get; } = Exception;
}