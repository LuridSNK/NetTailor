﻿using System.Linq.Expressions;
using HttTailor.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace HttTailor.Abstractions;

public interface IHttpRequestBuilder<TRequest>
{
    internal IServiceCollection Services { get; }
    
    public IHttpRequestBuilder<TRequest> Content(Expression<Func<TRequest, object>> configureContent, Naming? naming = Naming.CamelCase);
    
    public IHttpRequestBuilder<TRequest> Query(Expression<Func<TRequest, object>> configureQuery, Naming? naming = Naming.CamelCase);
    public IHttpRequestBuilder<TRequest> Headers(Expression<Action<TRequest, FluentHeaderDictionary>> configureHeaders);
    public IHttpRequestBuilder<TRequest> UseNaming(Naming naming);
}