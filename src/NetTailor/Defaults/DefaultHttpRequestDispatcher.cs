using System.Text.Json;
using NetTailor.Abstractions;
using NetTailor.Contracts;

namespace NetTailor.Defaults;

public class DefaultHttpRequestDispatcher : IHttpDispatcher
{
    private readonly IExecutionStrategyProvider _executionStrategyProvider;
    private readonly IHttpClientFactory _httpClientFactory;

    public DefaultHttpRequestDispatcher(IExecutionStrategyProvider executionStrategyProvider, IHttpClientFactory httpClientHttpClientFactory)
    {
        _executionStrategyProvider = executionStrategyProvider;
        _httpClientFactory = httpClientHttpClientFactory;
    }
    

    public async Task<HttpResult<TResponse>> Dispatch<TRequest, TResponse>(TRequest request, CancellationToken ct = default) 
        where TRequest : IHttpRequest<TResponse>
        where TResponse : class 
    {
        if (request == null) return HttpResults.Error<TResponse>(new ArgumentNullException(nameof(request)));

        var (strategy, client) = GetStrategyAndClient<TRequest, TResponse>(_executionStrategyProvider, _httpClientFactory);
       
        var httpResponseMessage = await SendCoreAsync(strategy, request, client, ct);
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            var httpMessage = $"API responded {httpResponseMessage.StatusCode:D} with reason phrase {httpResponseMessage.ReasonPhrase} while processing request {typeof(TRequest)}";
            var ex = new HttpRequestException(httpMessage);
            return HttpResults.Error<TResponse>(ex);
        }

        return await GetJsonContent(strategy, httpResponseMessage, ct);
    }

    public async Task<HttpResult<Empty>> Dispatch<TRequest>(TRequest request, CancellationToken ct = default) 
        where TRequest : IHttpRequest<Empty>
    {
        if (request == null) return HttpResults.Error(new ArgumentNullException(nameof(request)));

        var (strategy, client) = GetStrategyAndClient<TRequest, Empty>(_executionStrategyProvider, _httpClientFactory);
        
        var httpResponseMessage = await SendCoreAsync(strategy, request, client, ct);
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            var httpMessage = $"API responded {httpResponseMessage.StatusCode:D} with reason phrase {httpResponseMessage.ReasonPhrase} while processing request {typeof(TRequest)}";
            var ex = new HttpRequestException(httpMessage);
            return HttpResults.Error(ex);
        }
        
        return HttpResults.Success(Empty.Value);
    }
    
    private static (IExecutionStrategy<TRequest, TResponse>, HttpClient) GetStrategyAndClient<TRequest, TResponse>(
        IExecutionStrategyProvider executionStrategyProvider, 
        IHttpClientFactory httpClientFactory)
    {
        var strategy = executionStrategyProvider.Provide<TRequest, TResponse>();
        var client = httpClientFactory.CreateClient(strategy.ClientName);
        return (strategy, client);
    }
    
    private static async Task<HttpResponseMessage> SendCoreAsync<TRequest, TResponse>(
        IExecutionStrategy<TRequest, TResponse> strategy,
        TRequest request,
        HttpMessageInvoker client,
        CancellationToken ct = default)
    {
        var uri = await strategy.BuildEndpoint(request, ct);
        var message = new HttpRequestMessage(strategy.Method, uri)
        {
            Content = await strategy.BuildHttpContent(request, ct)
        };
        
        await strategy.BuildHeaders(request, message.Headers, ct);
        var responseMessage = await client.SendAsync(message, ct);
        return responseMessage;
    }

    private static async Task<HttpResult<TResponse>> GetJsonContent<TRequest, TResponse>(
        IExecutionStrategy<TRequest, TResponse> strategy, 
        HttpResponseMessage msg, 
        CancellationToken ct = default) where TResponse : class
    {
        var response = await strategy.Deserialize(msg.Content, ct);
        if (response is null)
        {
            var jsonMessage = $"Could not deserialize {typeof(TResponse)}. Yes, message of type {typeof(TRequest)} resulted in {msg.StatusCode:D} status code.";
            var ex = new JsonException(jsonMessage);
            return HttpResults.Error<TResponse>(ex);
        }

        return HttpResults.Success(response);
    }
}