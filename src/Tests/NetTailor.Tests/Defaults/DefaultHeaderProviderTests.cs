using System.Net.Http.Headers;
using FluentAssertions;
using NetTailor.Defaults;

namespace NetTailor.Tests.Defaults;

public class DefaultHeaderProviderTests
{
    [Theory]
    [InlineData("Bearer token123", "application/json")]
    [InlineData("Bearer abcdef", "text/plain")]
    public async Task HeaderProvider_Should_AddHeaders(string authorizationValue, string contentTypeValue)
    {
        // Arrange
        var request = new HttpRequestMessage();
        var headerProvider = new DefaultHeaderProvider<TestRequest>((r, h) =>
        {
            h.Authorization = new AuthenticationHeaderValue("Bearer", r.Authorization);
            h.Accept.Add(new MediaTypeWithQualityHeaderValue(r.ContentType));
        });

        var testRequest = new TestRequest { Authorization = authorizationValue, ContentType = contentTypeValue };

        // Act
        await headerProvider.Provide(testRequest, request.Headers);

        // Assert
        request.Headers.Authorization.Should().NotBeNull();
        request.Headers.Authorization.Scheme.Should().Be("Bearer");
        request.Headers.Authorization.Parameter.Should().Be(authorizationValue);

        request.Headers.Accept.Should().NotBeNull();
        request.Headers.Accept.Should().ContainSingle();
        request.Headers.Accept.First().MediaType.Should().Be(contentTypeValue);
    }

    
    [Fact]
    public async Task HeaderProvider_Should_NotAddHeaders_When_ConfigureHeadersDoesNothing()
    {
        // Arrange
        var request = new HttpRequestMessage();
        var headerProvider = new DefaultHeaderProvider<TestRequest>((r, h) => { });

        var testRequest = new TestRequest { Authorization = "Bearer token123", ContentType = "application/json" };

        // Act
        await headerProvider.Provide(testRequest, request.Headers);

        // Assert
        request.Headers.Authorization.Should().BeNull();
        request.Headers.Accept.Should().BeEmpty();
    }

    [Fact]
    public async Task HeaderProvider_Should_OverrideExistingHeaders_When_HeaderExists()
    {
        // Arrange
        var request = new HttpRequestMessage();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "old-token");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

        var headerProvider = new DefaultHeaderProvider<TestRequest>((r, h) =>
        {
            h.Authorization = new AuthenticationHeaderValue("Bearer", r.Authorization);
            h.Accept.Clear();
            h.Accept.Add(new MediaTypeWithQualityHeaderValue(r.ContentType));
        });

        var testRequest = new TestRequest { Authorization = "Bearer token123", ContentType = "application/json" };

        // Act
        await headerProvider.Provide(testRequest, request.Headers);

        // Assert
        request.Headers.Authorization.Should().NotBeNull();
        request.Headers.Authorization.Scheme.Should().Be("Bearer");
        request.Headers.Authorization.Parameter.Should().Be("Bearer token123");

        request.Headers.Accept.Should().NotBeNull();
        request.Headers.Accept.Should().ContainSingle();
        request.Headers.Accept.First().MediaType.Should().Be("application/json");
    }
    
    [Fact]
    public async Task Provider_ShouldThrowNullReference()
    {
        // Arrange
        var request = new HttpRequestMessage();
        var headerProvider = new DefaultHeaderProvider<TestRequest>(null);

        var testRequest = new TestRequest { Authorization = "Bearer token123", ContentType = "application/json" };

        // Act
        await FluentActions.Invoking(async () => await  headerProvider.Provide(testRequest, request.Headers))
            .Should().ThrowAsync<NullReferenceException>();
    }

    [Fact]
    public async Task HeaderProvider_ShouldThrow_When_HeaderValuesAreNotSet()
    {
        // Arrange
        var request = new HttpRequestMessage();
        var headerProvider = new DefaultHeaderProvider<TestRequest>((r, h) =>
        {
            h.Authorization = new AuthenticationHeaderValue("Bearer", r.Authorization);
            h.Accept.Add(new MediaTypeWithQualityHeaderValue(r.ContentType));
        });

        var testRequest = new TestRequest(); // properties are not set

        // Act
        await FluentActions.Invoking(async () => await  headerProvider.Provide(testRequest, request.Headers))
            .Should().ThrowAsync<ArgumentException>();
    }

    private class TestRequest
    {
        public string Authorization { get; set; }
        public string ContentType { get; set; }
    }
}