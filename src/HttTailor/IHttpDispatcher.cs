using HttTailor.Contracts;

namespace HttTailor;

public interface IHttpDispatcher
{
    public Task<HttpResult<TResponse>> Dispatch<TRequest, TResponse>(TRequest request, CancellationToken ct = default)
        where TRequest : IHttpRequest<TResponse>
        where TResponse : class;

    public Task<HttpResult<Empty>> Dispatch<TRequest>(TRequest request, CancellationToken ct = default)
        where TRequest : IHttpRequest<Empty>;
}