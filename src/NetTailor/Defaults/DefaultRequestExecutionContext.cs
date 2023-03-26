using NetTailor.Abstractions;

namespace NetTailor.Defaults;

internal record struct DefaultRequestExecutionContext<TRequest, TResponse>(
    HttpClient Client, 
    HttpMethod Method, 
    IEndpointBuilder<TRequest> EndpointBuilder, 
    IQueryStringBuilder<TRequest> QueryBuilder, 
    IHeaderProvider<TRequest> HeaderProvider, 
    IRequestBodyShaper<TRequest> BodyShaper, 
    IContentReader ContentReader, 
    IContentWriter ContentWriter) : IRequestExecutionContext<TRequest, TResponse>;