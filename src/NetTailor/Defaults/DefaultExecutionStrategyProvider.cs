using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NetTailor.Abstractions;
using NetTailor.Defaults.NoOp;

namespace NetTailor.Defaults;

public class DefaultExecutionStrategyProvider : IExecutionStrategyProvider
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultExecutionStrategyProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IExecutionStrategy<TRequest, TResponse> Provide<TRequest, TResponse>()
    {
        Debug.WriteLine($"[{typeof(TRequest)}:{typeof(TResponse)}]: Preparing execution strategy");
        var clientNameGetter = _serviceProvider.GetRequiredService<IClientNameGetter<TRequest>>();
        Debug.WriteLine($"[{typeof(TRequest)}:{typeof(TResponse)}]: Client name: {clientNameGetter.Name}");
        var httpMethodGetter = _serviceProvider.GetRequiredService<IHttpMethodGetter<TRequest>>();
        Debug.WriteLine($"[{typeof(TRequest)}:{typeof(TResponse)}]: Method: {httpMethodGetter.Method}");
        var endpointGetter = _serviceProvider.GetRequiredService<IEndpointBuilder<TRequest>>();
        var jsonOptionsGetter = _serviceProvider.GetService<IDefaultRequestNamingGetter<TRequest>>() ?? new DefaultRequestNamingGetter<TRequest>(Naming.CamelCase);
        var httpHeadersSetter = _serviceProvider.GetService<IHttpHeadersSetter<TRequest>>() ?? NoOpHeaderSetter<TRequest>.Instance;
        var queryStringBuilder = _serviceProvider.GetService<IQueryStringBuilder<TRequest>>() ?? NoOpQueryStringBuilder<TRequest>.Instance;
        var httpContentBuilder = _serviceProvider.GetService<IHttpContentBuilder<TRequest>>() ?? NoOpHttpContentBuilder<TRequest>.Instance;
        var responseDeserializer = _serviceProvider.GetRequiredService<IResponseDeserializer<TRequest, TResponse>>();
        return new DefaultExecutionStrategyFor<TRequest, TResponse>(
            clientNameGetter,
            httpMethodGetter,
            endpointGetter,
            jsonOptionsGetter,
            httpHeadersSetter,
            queryStringBuilder,
            httpContentBuilder,
            responseDeserializer);
    }
}