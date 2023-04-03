using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IO;
using NetTailor.Abstractions;
using NetTailor.Contracts;
using NetTailor.Defaults;
using NetTailor.Defaults.ContentSerializers;
using NetTailor.Defaults.NoOp;
using NetTailor.Extensions;
using NSubstitute;

namespace NetTailor.Tests.Defaults;

public class IRequestDispatcherTests
{
    private static readonly RecyclableMemoryStreamManager MemoryStreamManager = new();
    private readonly CamelCase _camelCase = new(MemoryStreamManager);

    

    [Fact]
    public async Task Dispatcher_ForGivenContext_ShouldDispatchRequestCorrectly()
    {
        IRequestExecutionContext<PostRequest, Empty> context = new DefaultRequestExecutionContext<PostRequest, Empty>(
            ClientName: "ExampleProfile",
            Method: HttpMethod.Post,
            EndpointBuilder: new DefaultEndpointBuilder<PostRequest>(r => $"/things/{r.Id}"),
            QueryBuilder: new NoOpQueryStringBuilder<PostRequest>(),
            HeaderProvider: NoOpHeaderProvider<PostRequest>.Instance,
            BodyShaper: NoOpRequestBodyShaper<PostRequest>.Instance,
            FormBuilder: NoOpFormBuilder<PostRequest>.Instance,
            ContentReader: _camelCase,
            ContentWriter: _camelCase
        );
        
        var contextFactory = Substitute.For<IRequestExecutionContextFactory>();
        contextFactory.Create<PostRequest, Empty>().Returns(_ => context);
        var clientFactory = Substitute.For<IHttpClientFactory>();
        clientFactory.CreateClient("ExampleProfile").Returns(_ => new HttpClient(new MockApi(_ => HttpStatusCode.NoContent))
        {
            BaseAddress = new Uri("https://api.example.com")
        });
        
        var logger = new NullLogger<DefaultRequestDispatcher>();
        IRequestDispatcher dispatcher = new DefaultRequestDispatcher(contextFactory, clientFactory, logger);
        var request = new PostRequest(69);
        var response = await dispatcher.Dispatch(request);
        response.Successful.Should().BeTrue();
    }
    
    [Fact]
    public async Task Dispatcher_ForGivenContext_ShouldDispatchRequestCorrectly_WhenUsingDependencyInjection()
    {
        var services = new ServiceCollection();
        services.AddHttpClientProfile("TestProfile",
            client => client.BaseAddress = new Uri("https://api.example.com"))
            .Post<PostRequest, Empty>(r => $"/things/{r.Id}")
            .AddHttpMessageHandler(() => new MockApi(_ => HttpStatusCode.NoContent));
        var sp = services.BuildServiceProvider();
        
        var dispatcher = sp.GetRequiredService<IRequestDispatcher>();
        var request = new PostRequest(69);
        var response = await dispatcher.Dispatch(request);
        response.Successful.Should().BeTrue();
    }
    public record PostRequest(int Id) : IHttpRequest<Empty>;
    [Fact]
    public async Task Dispatcher_ForGivenContext_ShouldDispatchRequest_WithBody()
    {
        RequestExecutionContextFactory.RequestExecutionContextCache<PostRequest, Empty>.Cache.Clear();
        var services = new ServiceCollection();
        services.AddHttpClientProfile("TestProfile",
                client => client.BaseAddress = new Uri("https://api.example.com"))
            .Post<PostRequest, Empty>(r => $"things/{r.Id}",
                rb =>
                {
                    rb.Content(r => new { Body = "Content" });
                    rb.Form(r => new { Form = "Content" });
                })
            .AddHttpMessageHandler(() => new MockApi(message =>
            {
                return message.Content switch
                {
                    MultipartFormDataContent => HttpStatusCode.BadRequest,
                    ByteArrayContent => HttpStatusCode.NoContent,
                    _ => HttpStatusCode.BadRequest
                };
            }));
        var sp = services.BuildServiceProvider();
        
        var dispatcher = sp.GetRequiredService<IRequestDispatcher>();
        var request = new PostRequest(69);
        var response = await dispatcher.Dispatch(request);
        response.Successful.Should().BeTrue();
    }
    
    [Fact]
    public async Task Dispatcher_ForGivenContext_ShouldDispatchRequest_WithMultipart()
    {
        RequestExecutionContextFactory.RequestExecutionContextCache<PostRequest, Empty>.Cache.Clear();
        var services = new ServiceCollection();
        services.AddHttpClientProfile("TestProfile",
                client => client.BaseAddress = new Uri("https://api.example.com"))
            .Post<PostRequest, Empty>(r => $"/things/{r.Id}",
                rb =>
                {
                    rb.Form(r => new { Form = "Content" });
                })
            .AddHttpMessageHandler(() => new MockApi(message =>
            {
                return message.Content switch
                {
                    MultipartFormDataContent => HttpStatusCode.NoContent,
                    _ => HttpStatusCode.BadRequest
                };
            }));
        var sp = services.BuildServiceProvider();
        
        var dispatcher = sp.GetRequiredService<IRequestDispatcher>();
        var request = new PostRequest(69);
        var response = await dispatcher.Dispatch(request);
        response.Successful.Should().BeTrue();
    }
}

public class MockApi : DelegatingHandler
{
    private readonly Func<HttpRequestMessage, HttpStatusCode> _configureCode;

    public MockApi(Func<HttpRequestMessage, HttpStatusCode> configure)
    {
        _configureCode = configure;
    }
    
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var code = _configureCode(request);
        return Task.FromResult(new HttpResponseMessage(code));
    }
}