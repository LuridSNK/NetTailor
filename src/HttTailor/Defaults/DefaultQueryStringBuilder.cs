using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using FastExpressionCompiler;
using HttTailor.Abstractions;
using HttTailor.Extensions;
using Microsoft.Extensions.ObjectPool;

namespace HttTailor.Defaults;

internal sealed class DefaultQueryStringBuilder<TRequest> : IQueryStringBuilder<TRequest>
{
    private readonly JsonNamingPolicy? _namingPolicy;
    private readonly Func<TRequest, object> _configureQuery;
    private readonly ObjectPool<StringBuilder> _stringBuilderObjectPool;

    public DefaultQueryStringBuilder(
        ObjectPool<StringBuilder> stringBuilderObjectPool, 
        Expression<Func<TRequest, object>> configureQuery,
        JsonNamingPolicy? namingPolicy = null)
    {
        _stringBuilderObjectPool = stringBuilderObjectPool;
        _configureQuery = configureQuery.CompileFast();
        _namingPolicy = namingPolicy;
    }
    
    public async ValueTask<string> Build(TRequest request)
    {
        var stringBuilder = _stringBuilderObjectPool.Get();
        var target = _configureQuery.Invoke(request);
        return QueryStringBuilder.Build(stringBuilder, target, _namingPolicy);
    }
}