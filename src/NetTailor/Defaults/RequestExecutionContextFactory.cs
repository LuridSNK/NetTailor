using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetTailor.Abstractions;
using NetTailor.Defaults.NoOp;

namespace NetTailor.Defaults;

public class RequestExecutionContextFactory : IRequestExecutionContextFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IContentWriterReaderFactory _contentWriterReaderFactory;
    private readonly ILoggerFactory _loggerFactory;

    public RequestExecutionContextFactory(
        IServiceProvider serviceProvider,
        IHttpClientFactory clientFactory,
        IContentWriterReaderFactory contentWriterReaderFactory, 
        ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _clientFactory = clientFactory;
        _contentWriterReaderFactory = contentWriterReaderFactory;
        _loggerFactory = loggerFactory;
    }

    public IRequestExecutionContext<TRequest, TResponse> Create<TRequest, TResponse>()
    {
        var logger = _loggerFactory.CreateLogger<RequestExecutionContextFactory>();
        using var scope = logger.BeginScope("ExecutionContext<{RequestType}, {ResponseType}>", typeof(TRequest), typeof(TResponse));
        var clientNameGetter = _serviceProvider.GetRequiredService<IClientNameGetter<TRequest>>();
        var requestNameGetter = _serviceProvider.GetRequiredService<IRequestNameGetter<TRequest>>();
        logger.LogDebug("Client: {ClientName}, Request: {RequestName}", clientNameGetter.Name, requestNameGetter.Name);
        if (RequestExecutionContextCache<TRequest, TResponse>.Cache
            .TryGetValue(requestNameGetter.Name, out var cachedCtx))
        {
            Debug.Assert(cachedCtx is not null);
            Debug.WriteLine($"Execution context has already been created for {requestNameGetter.Name}. Retuning from cache...");
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
        var formBuilder = _serviceProvider.GetService<IFormBuilder<TRequest>>()
                     ?? NoOpFormBuilder<TRequest>.Instance;
        var reader = _contentWriterReaderFactory.CreateReader(requestNameGetter.Name);
        var writer = _contentWriterReaderFactory.CreateWriter(requestNameGetter.Name);

        var executionCtx = new DefaultRequestExecutionContext<TRequest, TResponse>(
            Client: _clientFactory.CreateClient(clientNameGetter.Name),
            Method: methodNameGetter.Method,
            EndpointBuilder: endpointBuilder,
            QueryBuilder: queryBuilder,
            HeaderProvider: headerProvider,
            BodyShaper: bodyShaper,
            FormBuilder: formBuilder,
            ContentReader: reader,
            ContentWriter: writer
        );
        
        RequestExecutionContextCache<TRequest, TResponse>.Cache[requestNameGetter.Name] = executionCtx;
        Debug.WriteLine($"Successfully created new execution context for {requestNameGetter.Name}. Returning...");
        return executionCtx;
    }

    private static class RequestExecutionContextCache<TRequest, TResponse>
    {
        public static readonly IDictionary<string, IRequestExecutionContext<TRequest, TResponse>> Cache =
            new ConcurrentDictionary<string, IRequestExecutionContext<TRequest, TResponse>>();
    }
}