#nullable enable

namespace NetTailor.Contracts;

public record struct HttpResult<TResult>
(
    TResult? Value,
    bool Successful,
    Exception? Exception
);
