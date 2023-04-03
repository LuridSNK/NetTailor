namespace NetTailor.Abstractions;

/// <summary>
/// An execution content of the http request
/// </summary>
/// <typeparam name="TRequest">A request object which is used for sending to an api</typeparam>
/// <typeparam name="TResponse">A response object which is being returned from an api</typeparam>
public interface IRequestExecutionContext<TRequest, TResponse>
{
    /// <summary>
    /// <see cref="HttpClient"/> of request
    /// </summary>
    public string ClientName { get; }
    
    //public string HttpClientName { get; internal set; }
    
    /// <summary>
    /// A <see cref="HttpMethod"/> of the request
    /// </summary>
    public HttpMethod Method { get;  }
    
    /// <summary>
    /// An <see cref="IEndpointBuilder{TRequest}"/> of the request
    /// </summary>
    public IEndpointBuilder<TRequest> EndpointBuilder { get;  }
    
    /// <summary>
    /// A <see cref="IQueryStringBuilder{TRequest}"/> of the request
    /// </summary>
    public IQueryStringBuilder<TRequest> QueryBuilder { get;  }
    
    /// <summary>
    /// A <see cref="IHeaderProvider{TRequest}"/> of the request
    /// </summary>
    public IHeaderProvider<TRequest> HeaderProvider { get;  }
    
    /// <summary>
    /// A <see cref="IRequestBodyShaper{TRequest}"/> of the request
    /// </summary>
    public IRequestBodyShaper<TRequest> BodyShaper { get;  }
    
    /// <summary>
    /// A <see cref="IFormBuilder{TRequest}"/> of the request
    /// </summary>
    public IFormBuilder<TRequest> FormBuilder { get; }

    /// <summary>
    /// A <see cref="IRequestBodyShaper{TRequest}"/> of the while service
    /// </summary>
    public IContentReader ContentReader { get;  }
    
    /// <summary>
    /// A <see cref="IRequestBodyShaper{TRequest}"/> of the while service
    /// </summary>
    public IContentWriter ContentWriter { get; }
    
}