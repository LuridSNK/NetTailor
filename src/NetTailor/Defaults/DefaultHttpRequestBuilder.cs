using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using NetTailor.Abstractions;
using NetTailor.Defaults.ContentSerializers;
using NetTailor.ExpressionAnalysis;

namespace NetTailor.Defaults;

internal sealed class DefaultHttpRequestBuilder<TRequest, TResponse> : IHttpRequestBuilder<TRequest, TResponse>
{
    public DefaultHttpRequestBuilder(IServiceCollection services)
    {
        Services = services;
    }
    
    public IServiceCollection Services { get; }

    public IHttpRequestBuilder<TRequest, TResponse> Content(Expression<Func<TRequest, object>> configureContent)
    {
        if (configureContent == null) throw new ArgumentNullException(nameof(configureContent));
#if DEBUG
        ExpressionAnalyzer.Analyze(configureContent);
#endif
        Services.AddSingleton<IRequestBodyShaper<TRequest>, DefaultRequestBodyShaper<TRequest>>(
            _ => new DefaultRequestBodyShaper<TRequest>(configureContent));

        return this;
    }

    public IHttpRequestBuilder<TRequest, TResponse> Query(Expression<Func<TRequest, object>> configureQuery, Naming? naming = default)
    {
        if (configureQuery == null) throw new ArgumentNullException(nameof(configureQuery));
#if DEBUG
        ExpressionAnalyzer.Analyze(configureQuery);
#endif
        var namingPolicy = NamingPolicies.GetNamingPolicyOrDefault(naming);
        Services.AddSingleton<IQueryStringBuilder<TRequest>, DefaultQueryStringBuilder<TRequest>>(sp =>
        {
            var stringBuilderObjectPool = sp.GetRequiredService<ObjectPool<StringBuilder>>();
            return new DefaultQueryStringBuilder<TRequest>(
                stringBuilderObjectPool,
                configureQuery,
                namingPolicy);
        });
        return this;
    }

    public IHttpRequestBuilder<TRequest, TResponse> Headers(Action<TRequest, HttpRequestHeaders> configureHeaders)
    {
        if (configureHeaders == null) throw new ArgumentNullException(nameof(configureHeaders));
#if DEBUG
        ExpressionAnalyzer.AnalyzeAction(configureHeaders);
#endif
        Services.AddSingleton<IHeaderProvider<TRequest>, DefaultHeaderProvider<TRequest>>(
            _ => new DefaultHeaderProvider<TRequest>(configureHeaders));
        return this;
    }

    public IHttpRequestBuilder<TRequest, TResponse> UseContentReader<TContentReader>() where TContentReader : class, IContentReader
    
    {
        Debug.WriteLine(nameof(TRequest));
        Services.TryAddSingleton<TContentReader>();
        Services.Configure<ContentSerializerOptions>(nameof(TRequest), options =>
        {
            options.ContentReader = provider => provider.GetRequiredService<TContentReader>();
        });
        
        return this;
    }

    public IHttpRequestBuilder<TRequest, TResponse> UseContentWriter<TContentWriter>() where TContentWriter : class, IContentWriter
    
    {
        Debug.WriteLine(nameof(TRequest));
        Services.TryAddSingleton<TContentWriter>();
        Services.Configure<ContentSerializerOptions>(nameof(TRequest), options =>
        {
            options.ContentWriter = provider => provider.GetRequiredService<TContentWriter>();
        });
        
        return this;
    }
}
