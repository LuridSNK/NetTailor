using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NetTailor.Abstractions;
using NetTailor.Defaults.NoOp;

namespace NetTailor.Defaults;

public class DefaultRequestExecutionContextFactory : IRequestExecutionContextFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IContentWriterReaderFactory _contentWriterReaderFactory;

    public DefaultRequestExecutionContextFactory(
        IServiceProvider serviceProvider,
        IHttpClientFactory clientFactory,
        IContentWriterReaderFactory contentWriterReaderFactory)
    {
        _serviceProvider = serviceProvider;
        _clientFactory = clientFactory;
        _contentWriterReaderFactory = contentWriterReaderFactory;
    }

    public IRequestExecutionContext<TRequest, TResponse> Create<TRequest, TResponse>()
    {
        var clientNameGetter = _serviceProvider.GetRequiredService<IClientNameGetter<TRequest>>();
        var requestNameGetter = _serviceProvider.GetRequiredService<IRequestNameGetter<TRequest>>();
        if (RequestExecutionContextCache<TRequest, TResponse>.Cache
            .TryGetValue(requestNameGetter.Name, out var cachedCtx))
        {
            Debug.WriteLine($"[{clientNameGetter.Name}:{requestNameGetter.Name}] already exists in cache");
            return cachedCtx;
        }

        var methodNameGetter = _serviceProvider.GetRequiredService<IHttpMethodGetter<TRequest>>();
        var endpointBuilder = _serviceProvider.GetRequiredService<IEndpointBuilder<TRequest>>();

        var queryBuilder = _serviceProvider.GetService<IQueryStringBuilder<TRequest>>()
                           ?? NoOpQueryStringBuilder<TRequest>.Instance;

        var headerProvider = _serviceProvider.GetService<IHeaderProvider<TRequest>>()
                             ?? NoOpHeaderProvider<TRequest>.Instance;

        var bodyShaper = _serviceProvider.GetService<IRequestBodyShaper<TRequest>>()
                         ?? NoOpRequestBodyShaper<TRequest>.Instance;

        var reader = _contentWriterReaderFactory.CreateReader(requestNameGetter.Name);
        var writer = _contentWriterReaderFactory.CreateWriter(requestNameGetter.Name);

        var executionCtx = new DefaultRequestExecutionContext<TRequest, TResponse>(
            Client: _clientFactory.CreateClient(clientNameGetter.Name),
            Method: methodNameGetter.Method,
            EndpointBuilder: endpointBuilder,
            QueryBuilder: queryBuilder,
            HeaderProvider: headerProvider,
            BodyShaper: bodyShaper,
            ContentReader: reader,
            ContentWriter: writer
        );
        
        Debug.WriteLine($"Created [{clientNameGetter.Name}:{methodNameGetter.Method}:{requestNameGetter.Name}]");
        RequestExecutionContextCache<TRequest, TResponse>.Cache[requestNameGetter.Name] = executionCtx;
        return executionCtx;
    }

    private static class RequestExecutionContextCache<TRequest, TResponse>
    {
        public static readonly IDictionary<string, IRequestExecutionContext<TRequest, TResponse>> Cache =
            new ConcurrentDictionary<string, IRequestExecutionContext<TRequest, TResponse>>();
    }
}