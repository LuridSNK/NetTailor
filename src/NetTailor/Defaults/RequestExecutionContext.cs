using NetTailor.Abstractions;

namespace NetTailor.Defaults;

public record DefaultRequestExecutionContext<TRequest, TResponse>(
    HttpClient Client, 
    HttpMethod Method, 
    IEndpointBuilder<TRequest> EndpointBuilder, 
    IQueryStringBuilder<TRequest> QueryBuilder, 
    IHeaderProvider<TRequest> HeaderProvider, 
    IRequestBodyShaper<TRequest> BodyShaper, 
    IContentReader ContentReader, 
    IContentWriter ContentWriter) : IRequestExecutionContext<TRequest, TResponse>
{
    public HttpClient Client { get; } = Client;
    public HttpMethod Method { get; } = Method;
    public IEndpointBuilder<TRequest> EndpointBuilder { get; } = EndpointBuilder;
    public IQueryStringBuilder<TRequest> QueryBuilder { get; } = QueryBuilder;
    public IHeaderProvider<TRequest> HeaderProvider { get; } = HeaderProvider;
    public IRequestBodyShaper<TRequest> BodyShaper { get; } = BodyShaper;
    public IContentReader ContentReader { get; } = ContentReader;
    public IContentWriter ContentWriter { get; } = ContentWriter;
}