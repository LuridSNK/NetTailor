using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Microsoft.IO;
using NetTailor.Abstractions;
using NetTailor.Contracts;
using NetTailor.Defaults.ContentSerializers;
using NetTailor.Defaults.NoOp;
using NetTailor.Extensions;

namespace NetTailor.Tests;

public class DependencyInjectionTests
{
    private static IServiceCollection Services => new ServiceCollection();

    [Fact]
    public void ServiceCollection_WhenNamedProfileAdded_ShouldContainRequiredDependencies()
    {
        var services = Services;
        services.AddHttpClientProfile("TestProfile",
            client => { client.BaseAddress = new Uri("https://api.example.com"); });

        // required
        services.Should().Contain(s => s.ServiceType == typeof(IRequestExecutionContextFactory));
        services.Should().Contain(s => s.ServiceType == typeof(IRequestDispatcher));
        services.Should().Contain(s => s.ServiceType == typeof(IContentWriterReaderFactory));

        // secondary
        services.Should().Contain(s => s.ServiceType == typeof(RecyclableMemoryStreamManager));
        services.Should().Contain(s => s.ServiceType == typeof(ObjectPool<StringBuilder>));
        services.Should().Contain(s => s.ServiceType == typeof(IContentTypeProvider));
        services.Should().Contain(s => s.ServiceType == typeof(CamelCase));
    }

    [Fact]
    public void ServiceCollection_WhenUnnamedProfileAdded_ShouldContainRequiredDependencies()
    {
        var services = Services;
        services.AddHttpClientProfile(client => { client.BaseAddress = new Uri("https://api.example.com"); });

        // required
        services.Should().Contain(s => s.ServiceType == typeof(IRequestExecutionContextFactory));
        services.Should().Contain(s => s.ServiceType == typeof(IRequestDispatcher));
        services.Should().Contain(s => s.ServiceType == typeof(IContentWriterReaderFactory));

        // secondary
        services.Should().Contain(s => s.ServiceType == typeof(RecyclableMemoryStreamManager));
        services.Should().Contain(s => s.ServiceType == typeof(ObjectPool<StringBuilder>));
        services.Should().Contain(s => s.ServiceType == typeof(IContentTypeProvider));
        services.Should().Contain(s => s.ServiceType == typeof(CamelCase));
    }
    
    internal class StronglyTypedProfile : IHttpServiceProfile
    {
        public void Configure(IHttpServiceBuilder builder)
        {
            builder.Create("TestStronglyTypedProfile", _ => {})
                .Post<PostRequest, Empty>(route => $"/things/{route.Id}");;
        }
    }
    
    [Fact]
    public void ServiceCollection_WhenStronglyTypedProfileWithRequestAdded_ShouldContainRequiredDependencies()
    {
        var services = Services;
        services.AddHttpClientProfile<StronglyTypedProfile>();

        // required
        services.Should().Contain(s => s.ServiceType == typeof(IRequestExecutionContextFactory));
        services.Should().Contain(s => s.ServiceType == typeof(IRequestDispatcher));
        services.Should().Contain(s => s.ServiceType == typeof(IContentWriterReaderFactory));

        // secondary
        services.Should().Contain(s => s.ServiceType == typeof(RecyclableMemoryStreamManager));
        services.Should().Contain(s => s.ServiceType == typeof(ObjectPool<StringBuilder>));
        services.Should().Contain(s => s.ServiceType == typeof(IContentTypeProvider));
        services.Should().Contain(s => s.ServiceType == typeof(CamelCase));
        
        // request essentials
        services.Should().Contain(s => s.ServiceType == typeof(IRequestNameGetter<PostRequest>));
        services.Should().Contain(s => s.ServiceType == typeof(IClientNameGetter<PostRequest>));
        services.Should().Contain(s => s.ServiceType == typeof(IHttpMethodGetter<PostRequest>));
        services.Should().Contain(s => s.ServiceType == typeof(IEndpointBuilder<PostRequest>));
    }
    
    
    [Fact]
    public void ServiceProvider_WhenRequestingADispatcher_ShouldNotThrow()
    {
        var services = Services;
        services.AddHttpClientProfile(client => { client.BaseAddress = new Uri("https://api.example.com"); });
        var serviceProvider = services.BuildServiceProvider();

        FluentActions.Invoking(() => serviceProvider.GetRequiredService<IRequestDispatcher>()).Should().NotThrow();
    }


    private record PostRequest(int Id) : IHttpRequest<Empty>;

    [Fact]
    public void RequestExecutionContextFactory_WhenCreatingAConfiguredRequest_ShouldCreateAValidStrategy()
    {
        var services = Services;
        services.AddHttpClientProfile(client => { client.BaseAddress = new Uri("https://api.example.com"); })
            .Post<PostRequest, Empty>(route => $"/things/{route.Id}");

        var serviceProvider = services.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<IRequestExecutionContextFactory>();
        var executionStrategy = factory.Create<PostRequest, Empty>();
        executionStrategy.Should().NotBeNull();
    }
    
    [Fact]
    public void RequestExecutionContextFactory_WhenCreatingANotConfiguredRequest_ShouldThrowWithInvalidOperation()
    {
        var services = Services;
        services.AddHttpClientProfile(client => { client.BaseAddress = new Uri("https://api.example.com"); });
        
        var serviceProvider = services.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<IRequestExecutionContextFactory>();
        FluentActions.Invoking(() => factory.Create<PostRequest, Empty>())
            .Should().Throw<InvalidOperationException>();
    }
    
    [Fact]
    public void CreatedStrategy_ShouldBe()
    {
        var services = Services;
        services.AddHttpClientProfile(client => { client.BaseAddress = new Uri("https://api.example.com"); })
            .Post<PostRequest, Empty>(route => $"/things/{route.Id}");

        var serviceProvider = services.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<IRequestExecutionContextFactory>();
        var executionStrategy = factory.Create<PostRequest, Empty>();

        var actualRequest = new PostRequest(420);
        
        // in: .Post<PostRequest, Empty>
        executionStrategy!.Method.Should().Be(HttpMethod.Post);
        
        // in: route => $"/things/{route.Id}"
        var endpoint = executionStrategy.EndpointBuilder.Build(actualRequest).GetAwaiter().GetResult();
        endpoint.Should().BeEquivalentTo("/things/420");
        
        // since not body were configured it should be a NoOp
        executionStrategy.BodyShaper.Should().BeOfType<NoOpRequestBodyShaper<PostRequest>>();
       
        // since not content type were configured it should be a NoOp
        executionStrategy.HeaderProvider.Should().BeOfType<NoOpHeaderProvider<PostRequest>>();
        
        // since not query string were configured it should be a NoOp
        executionStrategy.QueryBuilder.Should().BeOfType<NoOpQueryStringBuilder<PostRequest>>();
    }
}