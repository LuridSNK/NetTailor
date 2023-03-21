﻿using System.Linq.Expressions;
using FastExpressionCompiler;
using HttTailor.Abstractions;

namespace HttTailor.Defaults;

internal sealed class DefaultEndpointBuilder<TRequest> : IEndpointBuilder<TRequest> 
{
    private readonly Func<TRequest, string> _routeBuilder;
    public DefaultEndpointBuilder(Expression<Func<TRequest, string>> routeConfiguration)
    {
        _routeBuilder = routeConfiguration.CompileFast();
    }

    public async ValueTask<string> Build(TRequest request)
    {
        return _routeBuilder.Invoke(request);
    }
}