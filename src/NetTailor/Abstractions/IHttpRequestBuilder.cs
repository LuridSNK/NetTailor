using System.Linq.Expressions;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;

namespace NetTailor.Abstractions;

public interface IHttpRequestBuilder<TRequest>
{
    internal IServiceCollection Services { get; }
    
    public IHttpRequestBuilder<TRequest> Content(Expression<Func<TRequest, object>> configureContent, Naming? naming = Naming.CamelCase);
    
    public IHttpRequestBuilder<TRequest> Query(Expression<Func<TRequest, object>> configureQuery, Naming? naming = Naming.CamelCase);
    public IHttpRequestBuilder<TRequest> Headers(Action<TRequest, HttpRequestHeaders> configureHeaders);
    public IHttpRequestBuilder<TRequest> UseNaming(Naming naming);
}