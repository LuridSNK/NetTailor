namespace HttTailor.Contracts;

public static class HttpResults
{
    public static HttpResult<TResult> Error<TResult>(Exception ex) where TResult : class
    {
        return new HttpResult<TResult>(null, false, ex);
    }
    
    public static HttpResult<TResult> Success<TResult>(TResult result)
    {
        return new HttpResult<TResult>(result, true, null);
    }
    
    public static HttpResult<Empty> Empty()
    {
        return new HttpResult<Empty>(Contracts.Empty.Value, true, null);
    }
    
    public static HttpResult<Empty> Error(Exception ex)
    {
        return new HttpResult<Empty>(Contracts.Empty.Value, true, null);
    }
}