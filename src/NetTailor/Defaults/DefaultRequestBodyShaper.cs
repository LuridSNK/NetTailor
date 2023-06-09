﻿using System.Linq.Expressions;
using FastExpressionCompiler;
using NetTailor.Abstractions;

namespace NetTailor.Defaults;

internal class DefaultRequestBodyShaper<TRequest> : IRequestBodyShaper<TRequest>
{
    private readonly Func<TRequest, object> _shapeBody;

    public DefaultRequestBodyShaper(Expression<Func<TRequest,object>> shapeBody)
    {
        _shapeBody = shapeBody.CompileFast();
    }

    public object? Shape(TRequest request) => _shapeBody.Invoke(request);
}
