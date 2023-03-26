namespace NetTailor.Contracts;

public static class HttpResults
{
    /// <summary>
    /// Creates a new instance of <see cref="HttpResult{TResult}"/> with the <see cref="Exception"/> as an Failure.
    /// </summary>
    public static HttpResult<TResult> Failure<TResult>(Exception ex) where TResult : class
    {
        return new HttpResult<TResult>(null, false, ex);
    }

    /// <summary>
    /// Creates a new instance of <see cref="HttpResult{TResult}"/> with the specified result.
    /// </summary>
    public static HttpResult<TResult> Success<TResult>(TResult result)
    {
        return new HttpResult<TResult>(result, true, null);
    }

    /// <summary>
    /// Creates a new instance of <see cref="HttpResult{TResult}"/> with the <see cref="Empty"/> result.
    /// </summary>
    public static HttpResult<Empty> Empty()
    {
        return new HttpResult<Empty>(Contracts.Empty.Value, true, null);
    }

    /// <summary>
    /// Creates a new instance of <see cref="HttpResult{TResult}"/> with the <see cref="Exception"/> as an Failure.
    /// </summary>
    public static HttpResult<Empty> Failure(Exception ex)
    {
        return new HttpResult<Empty>(Contracts.Empty.Value, false, ex);
    }
}