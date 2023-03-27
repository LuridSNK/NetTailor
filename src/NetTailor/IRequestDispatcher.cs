using NetTailor.Contracts;

namespace NetTailor;

public interface IRequestDispatcher
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public Task<HttpResult<TResponse?>> Dispatch<TRequest, TResponse>(TRequest request, CancellationToken ct = default)
        where TRequest : IRequest<TResponse>
        where TResponse : class;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <typeparam name="TRequest"></typeparam>
    /// <returns></returns>
    public Task<HttpResult<Empty>> Dispatch<TRequest>(TRequest request, CancellationToken ct = default)
        where TRequest : IRequest<Empty>;
}