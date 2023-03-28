using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using FastExpressionCompiler;
using NetTailor.Abstractions;

namespace NetTailor.Defaults;

internal sealed class DefaultEndpointBuilder<TRequest> : IEndpointBuilder<TRequest>
{
    private readonly Func<TRequest, string> _routeBuilder;

    public DefaultEndpointBuilder(Expression<Func<TRequest, string>> routeConfiguration)
    {
        _routeBuilder = routeConfiguration.CompileFast();
    }

    public ValueTask<string> Build(TRequest request, CancellationToken ct = default)
    {
        var endpoint = _routeBuilder.Invoke(request);
        return new ValueTask<string>(endpoint);
    }
}