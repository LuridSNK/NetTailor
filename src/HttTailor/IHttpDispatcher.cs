using HttTailor.Contracts;

namespace HttTailor;

public interface IHttpDispatcher
{
    public Task<HttpResult<TResponse>> Send<TRequest, TResponse>(TRequest request, CancellationToken ct = default)
        where TRequest : IHttpRequest<TResponse>
        where TResponse : class;

    public Task<HttpResult<Empty>> Send<TRequest>(TRequest request, CancellationToken ct = default)
        where TRequest : IHttpRequest<Empty>;
}