using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NetTailor.Abstractions;
using NetTailor.Contracts;

namespace NetTailor.Defaults;

public class DefaultRequestDispatcher : IRequestDispatcher
{
    private readonly IRequestExecutionContextFactory _contextFactory;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<DefaultRequestDispatcher> _logger;

    public DefaultRequestDispatcher(
        IRequestExecutionContextFactory contextFactory, 
        IHttpClientFactory clientFactory, 
        ILogger<DefaultRequestDispatcher> logger)
    {
        _contextFactory = contextFactory;
        _clientFactory = clientFactory;
        _logger = logger;
    }
    
    public async Task<HttpResult<TResponse?>> Dispatch<TRequest, TResponse>(TRequest request, CancellationToken ct = default) 
        where TRequest : IHttpRequest<TResponse>
        where TResponse : class 
    {
        if (request == null) return HttpResults.Failure<TResponse>(new ArgumentNullException(nameof(request)));
        
        var ctx = _contextFactory.Create<TRequest, TResponse>();
        
        if (ctx is null) return HttpResults.Failure<TResponse>(new NullReferenceException(nameof(ctx)));
        var uri = await BuildUri(ctx, request, ct);
        var message = ctx.BodyShaper.Shape(request);
        var content = await ctx.ContentWriter.Write(message, ct) ?? await ctx.FormBuilder.Build(request);
        var httpRequest = new HttpRequestMessage(ctx.Method, new Uri(uri, UriKind.Relative))
        {
            Content = content
        };
        
        await ctx.HeaderProvider.Provide(request, httpRequest.Headers, ct);
#if DEBUG
        _logger.LogDebug("Message sent: {Message}", httpRequest);
#endif
        var client = _clientFactory.CreateClient(ctx.ClientName);
        var httpResponse = await client.SendAsync(httpRequest, ct);
        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Remote resource {Api} responded with {Code}:{ReasonPhrase} while processing request {Request}", 
                httpResponse.RequestMessage?.RequestUri, 
                httpResponse.StatusCode, 
                httpResponse.ReasonPhrase, 
                typeof(TRequest));
            var httpMessage = $"Remote resource responded {httpResponse.StatusCode:D} with reason phrase {httpResponse.ReasonPhrase} while processing request {typeof(TRequest)}";
#if ASPNETCORE
            var ex = new HttpRequestException(httpMessage, statusCode: httpResponse.StatusCode);
#else
            var ex = new HttpRequestException(httpMessage);
#endif
            return HttpResults.Failure<TResponse>(ex);
        }
#if DEBUG
        _logger.LogDebug("Message received: {Message}", httpResponse);
#endif
        var value = await ctx.ContentReader.Read<TResponse>(httpResponse.Content!, ct);
        
        if (value is null)
        {
            _logger.LogWarning("Could not deserialize type {Type} from response {Response}", typeof(TResponse), httpResponse);
            var jsonMessage = $"Could not deserialize {typeof(TResponse)}. Yes, message of type {typeof(TRequest)} resulted in {httpResponse.StatusCode:D} status code.";
            var ex = new JsonException(jsonMessage);
            return HttpResults.Failure<TResponse>(ex);
        }
        
        return HttpResults.Success(value);
    }

    public async Task<HttpResult<Empty>> Dispatch<TRequest>(TRequest request, CancellationToken ct = default) 
        where TRequest : IHttpRequest<Empty>
    {
        if (request == null) return HttpResults.Failure(new ArgumentNullException(nameof(request)));
        
        var ctx = _contextFactory.Create<TRequest, Empty>();
        
        if (ctx is null) return HttpResults.Failure(new ArgumentNullException(nameof(ctx)));
        var uri = await BuildUri(ctx, request, ct);
        var message = ctx.BodyShaper.Shape(request);
        var content = await ctx.ContentWriter.Write(message, ct) ?? await ctx.FormBuilder.Build(request);
        var httpRequest = new HttpRequestMessage(ctx.Method, new Uri(uri, UriKind.Relative))
        {
            Content = content
        };
        
        await ctx.HeaderProvider.Provide(request, httpRequest.Headers, ct);
#if DEBUG
        _logger.LogDebug("Message sent: {Message}", httpRequest);
#endif
        var client = _clientFactory.CreateClient(ctx.ClientName);
        var httpResponse = await client.SendAsync(httpRequest, ct);
        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Remote resource {Api} responded with {Code}:{ReasonPhrase} while processing request {Request}", 
                httpResponse.RequestMessage?.RequestUri, 
                httpResponse.StatusCode, 
                httpResponse.ReasonPhrase, 
                typeof(TRequest));
            var httpMessage = $"Remote resource responded {httpResponse.StatusCode:D} with reason phrase {httpResponse.ReasonPhrase} while processing request {typeof(TRequest)}";
#if ASPNETCORE
            var ex = new HttpRequestException(httpMessage, statusCode: httpResponse.StatusCode);
#else
            var ex = new HttpRequestException(httpMessage);
#endif
            return HttpResults.Failure(ex);
        }
#if DEBUG
        _logger.LogDebug("Message received: {Message}", httpResponse);
#endif
        
        return HttpResults.Success(Empty.Value);
    }

    private static async ValueTask<string> BuildUri<TRequest, TResponse>(IRequestExecutionContext<TRequest, TResponse> ctx, TRequest request, CancellationToken ct = default)
    {
        var endpointString = await ctx.EndpointBuilder.Build(request, ct);
        var queryString = await ctx.QueryBuilder.Build(request, ct);
        
        Debug.WriteLine($"[{typeof(TRequest)}:{typeof(TResponse)}] endpoint: {endpointString}");
        Debug.WriteLine($"[{typeof(TRequest)}:{typeof(TResponse)}] query: {queryString}");

        return string.Concat(endpointString, queryString);
    }
}