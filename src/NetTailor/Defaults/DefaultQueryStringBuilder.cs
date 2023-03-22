using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using FastExpressionCompiler;
using Microsoft.Extensions.ObjectPool;
using NetTailor.Abstractions;
using NetTailor.Extensions;

namespace NetTailor.Defaults;

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
    
    public ValueTask<string> Build(TRequest request)
    {
        var stringBuilder = _stringBuilderObjectPool.Get();
        var target = _configureQuery.Invoke(request);
        var queryString =  QueryStringBuilder.Build(stringBuilder, target, _namingPolicy);
        return new ValueTask<string>(queryString);
    }
}