using System.Diagnostics;
using System.Text.Json;
using NetTailor.Abstractions;
using NetTailor.Contracts;

namespace NetTailor.Defaults;

public class DefaultRequestDispatcher : IRequestDispatcher
{
    private readonly IRequestExecutionContextFactory _contextFactory;

    public DefaultRequestDispatcher(IRequestExecutionContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public async Task<HttpResult<TResponse?>> Dispatch<TRequest, TResponse>(TRequest request, CancellationToken ct = default) 
        where TRequest : IRequest<TResponse>
        where TResponse : class 
    {
        if (request == null) return HttpResults.Failure<TResponse>(new ArgumentNullException(nameof(request)));
        
        var ctx = _contextFactory.Create<TRequest, TResponse>();
        
        if (ctx is null) return HttpResults.Failure<TResponse>(new ArgumentNullException(nameof(ctx)));

        var uri = await BuildUri(ctx, request, ct);
        var message = ctx.BodyShaper.Shape(request);
        var content = await ctx.ContentWriter.Write(message, ct);
        var httpRequest = new HttpRequestMessage(ctx.Method, uri)
        {
            Content = content
        };
        
        await ctx.HeaderProvider.Provide(request, httpRequest.Headers, ct);
        
        Debug.WriteLine(httpRequest);
        
        var httpResponse = await ctx.Client.SendAsync(httpRequest, ct);
        if (!httpResponse.IsSuccessStatusCode)
        {
            var httpMessage = $"API responded {httpResponse.StatusCode:D} with reason phrase {httpResponse.ReasonPhrase} while processing request {typeof(TRequest)}";
            var ex = new HttpRequestException(httpMessage);
            return HttpResults.Failure<TResponse>(ex);
        }
        
        Debug.WriteLine(httpResponse);
        
        var value = await ctx.ContentReader.Read<TResponse>(httpResponse.Content!, ct);
        
        if (value is null)
        {
            var jsonMessage = $"Could not deserialize {typeof(TResponse)}. Yes, message of type {typeof(TRequest)} resulted in {httpResponse.StatusCode:D} status code.";
            var ex = new JsonException(jsonMessage);
            return HttpResults.Failure<TResponse>(ex);
        }
        
        return HttpResults.Success(value);
    }

    public async Task<HttpResult<Empty>> Dispatch<TRequest>(TRequest request, CancellationToken ct = default) 
        where TRequest : IRequest<Empty>
    {
        if (request == null) return HttpResults.Failure(new ArgumentNullException(nameof(request)));
        
        var ctx = _contextFactory.Create<TRequest, Empty>();
        
        if (ctx is null) return HttpResults.Failure(new ArgumentNullException(nameof(ctx)));

        var uri = await BuildUri(ctx, request, ct);
        var message = new HttpRequestMessage(ctx.Method, uri)
        {
            Content = await ctx.ContentWriter.Write(request, ct)
        };
        
        await ctx.HeaderProvider.Provide(request, message.Headers, ct);
        
        Debug.WriteLine(message);
        Debug.WriteLineIf(message.Content is not null, message.Content!.ReadAsStringAsync());
        
        var httpResponse = await ctx.Client.SendAsync(message, ct);
        if (!httpResponse.IsSuccessStatusCode)
        {
            var httpMessage = $"API responded {httpResponse.StatusCode:D} with reason phrase {httpResponse.ReasonPhrase} while processing request {typeof(TRequest)}";
            var ex = new HttpRequestException(httpMessage);
            return HttpResults.Failure(ex);
        }
        
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