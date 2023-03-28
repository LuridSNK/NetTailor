using FluentAssertions;
using NetTailor.Defaults;

namespace NetTailor.Tests.Defaults;

public class DefaultEndpointBuilderTests
{
    private record ExampleRequest(int? Id);
    
    [Theory]
    [InlineData(12)]
    [InlineData(23)]
    [InlineData(34)]
    [InlineData(45)]
    [InlineData(56)]
    public async Task EndpointBuilder_Should_ReturnCorrectEndpoint(int id)
    {
        var request = new ExampleRequest(id);
        var builder = new DefaultEndpointBuilder<ExampleRequest>(r => $"example/{r.Id}");

        var endpoint = await builder.Build(request);

        endpoint.Should().Be($"example/{id}");
    }

    [Theory]
    [InlineData(12)]
    [InlineData(23)]
    [InlineData(34)]
    [InlineData(45)]
    [InlineData(56)]
    public async Task EndpointBuilder_Should_ReturnSameEndpoint_ForSameRequest(int id)
    {
        var request = new ExampleRequest(id);
        var builder = new DefaultEndpointBuilder<ExampleRequest>(r => $"example/{r.Id}");

        var endpoint1 = await builder.Build(request);
        var endpoint2 = await builder.Build(request);

        endpoint1.Should().BeEquivalentTo(endpoint2);
    }
    
    [Fact]
    public async Task EndpointBuilder_WhenRequestIsNull_ShouldThrowArgumentNullException()
    {
        var builder = new DefaultEndpointBuilder<ExampleRequest>(r => $"example/{r.Id}");
        
        // still viable, since the library checks the request value in the dispatcher
        FluentActions.Invoking(() => builder.Build(null))
            .Should().Throw<NullReferenceException>();
    }
    
    [Fact]
    public async Task EndpointBuilder_Should_ReturnDifferentEndpoints_ForDifferentRequests()
    {
        var builder = new DefaultEndpointBuilder<ExampleRequest>(r => $"example/{r.Id}");

        var endpoint1 = await builder.Build(new ExampleRequest(123));
        var endpoint2 = await builder.Build(new ExampleRequest(456));

        endpoint1.Should().NotBeEquivalentTo(endpoint2);
    }

    [Fact]
    public async Task EndpointBuilder_Should_HandleRequestsWithNullId()
    {
        var builder = new DefaultEndpointBuilder<ExampleRequest>(r => $"example/{r.Id}");

        var endpoint = await builder.Build(new ExampleRequest((int?)null));
        
        // this is viable, since the library does not own user's requests
        Assert.Equal("example/", endpoint); 
    }

    [Fact]
    public async Task EndpointBuilder_ShouldHandleCancellation_WithoutThrowing()
    {
        // Arrange
        var builder = new DefaultEndpointBuilder<ExampleRequest>(r => $"example/{r.Id}");
        var request = new ExampleRequest(123);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await FluentActions.Invoking(async () =>
        {
            await builder.Build(request, cts.Token);
        })
            .Should()
            .NotThrowAsync();
    }
}