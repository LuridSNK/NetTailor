using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Microsoft.IO;
using NetTailor.Abstractions;
using NetTailor.Contracts;
using NetTailor.Extensions;

namespace NetTailor.Defaults;

internal sealed class DefaultHttpRequestBuilder<TRequest> : IHttpRequestBuilder<TRequest>
{
    public DefaultHttpRequestBuilder(IServiceCollection services) => Services = services;
    public IServiceCollection Services { get; }
    
    public IHttpRequestBuilder<TRequest> Content(Expression<Func<TRequest, object>> configureContent, Naming? naming = Naming.CamelCase)
    {
        if (configureContent == null) throw new ArgumentNullException(nameof(configureContent));
        
        var jsonOptions = JsonSerializerOptionsCache.GetSettingsOrDefault(naming);
        Debug.WriteLine($"Naming policy for request Content [{typeof(TRequest)}] is {naming:G}");
        Services.AddSingleton<IHttpContentBuilder<TRequest>, DefaultHttpContentBuilder<TRequest>>(sp =>
        {
            var memoryStreamManager = sp.GetRequiredService<RecyclableMemoryStreamManager>();
            return new DefaultHttpContentBuilder<TRequest>(
                memoryStreamManager, 
                configureContent, 
                jsonOptions);
        });
        return this;
    }

    public IHttpRequestBuilder<TRequest> Query(Expression<Func<TRequest, object>> configureQuery, Naming? naming = Naming.CamelCase)
    {
        if (configureQuery == null) throw new ArgumentNullException(nameof(configureQuery));
        
        var namingPolicy = NamingPolicies.GetNamingPolicyOrDefault(naming);
        Debug.WriteLine($"Naming policy for request Query [{typeof(TRequest)}] is {naming:G}");
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

    public IHttpRequestBuilder<TRequest> Headers(Action<TRequest, HttpRequestHeaders> configureHeaders)
    {
        if (configureHeaders == null) throw new ArgumentNullException(nameof(configureHeaders));
        
        Services.AddSingleton<IHttpHeadersSetter<TRequest>, DefaultHttpHeadersSetter<TRequest>>(
            _ => new DefaultHttpHeadersSetter<TRequest>(configureHeaders));
        return this;
    }

    public IHttpRequestBuilder<TRequest> UseNaming(Naming naming)
    {
        JsonSerializerOptionsCache.GetSettingsOrDefault(naming);
        Services.AddSingleton<IDefaultRequestNamingGetter<TRequest>, DefaultRequestNamingGetter<TRequest>>(
            _ => new DefaultRequestNamingGetter<TRequest>(naming));
        return this;
    }
}