using System.Diagnostics;
using System.Net.Http.Headers;
using FluentAssertions;
using NetTailor.Abstractions;
using NetTailor.Defaults;
using NetTailor.Utilities;

namespace NetTailor.Tests;

public class HeaderProviderTests
{
    private record FakeRequest(Guid? GuidValue, string StringValue, int? NumericValue, string[] StringArrayValue)
    {
        public static FakeRequest Instance => new(
            GuidValue: Guid.NewGuid(),
            StringValue: RandomNames.Generate(),
            NumericValue: Random.Shared.Next(),
            StringArrayValue: Enumerable.Range(1, 5).Select(_ => RandomNames.Generate()).ToArray());
    }

    [Fact]
    public async ValueTask HeaderValueProvider_WhenGivenARequest_ShouldWriteAllValuesInToHeaders()
    {
        var randomizedRequest = FakeRequest.Instance;

        Action<FakeRequest, HttpRequestHeaders> providerDelegate = (fakeRequest, headers) =>
        {
            headers.Add(nameof(fakeRequest.GuidValue), fakeRequest.GuidValue?.ToString("N"));
            headers.Add(nameof(fakeRequest.StringValue), fakeRequest.StringValue);
            headers.Add(nameof(fakeRequest.NumericValue), fakeRequest.NumericValue?.ToString());
            headers.Add(nameof(fakeRequest.StringArrayValue), fakeRequest.StringArrayValue);
        };

        IHeaderProvider<FakeRequest> headerProvider = new DefaultHeaderProvider<FakeRequest>(providerDelegate);

        using var message = new HttpRequestMessage();
        
        await headerProvider.Provide(
            request: randomizedRequest,
            headers: message.Headers);
        
        var guid = message.Headers.GetValues(nameof(randomizedRequest.GuidValue)).First();
        Guid.Parse(guid).Should().Be(randomizedRequest.GuidValue!.Value);
        var strValue = message.Headers.GetValues(nameof(randomizedRequest.StringValue)).First();
        strValue.Should().Be(randomizedRequest.StringValue);
        var numeric = message.Headers.GetValues(nameof(randomizedRequest.NumericValue)).First();
        int.Parse(numeric).Should().Be(randomizedRequest.NumericValue);
        var array = message.Headers.GetValues(nameof(randomizedRequest.StringArrayValue));
        array.Should().Equal(randomizedRequest.StringArrayValue);
    }

    [Fact]
    public async ValueTask
        HeaderValueProvider_WhenGivenARequestWithNullValues_ShouldWriteStringEmptyValuesIntoThoseHeaders()
    {
        var randomizedRequest = new FakeRequest(null, null, null, null);

        Action<FakeRequest, HttpRequestHeaders> providerDelegate = (fakeRequest, headers) =>
        {
            headers.Add(nameof(fakeRequest.GuidValue), fakeRequest.GuidValue?.ToString("N"));
            headers.Add(nameof(fakeRequest.StringValue), fakeRequest.StringValue);
            headers.Add(nameof(fakeRequest.NumericValue), fakeRequest.NumericValue?.ToString());
            headers.Add(nameof(fakeRequest.StringArrayValue), fakeRequest.StringArrayValue);
        };

        IHeaderProvider<FakeRequest> headerProvider = new DefaultHeaderProvider<FakeRequest>(providerDelegate);

        using var message = new HttpRequestMessage();
        
        await headerProvider.Provide(
            request: randomizedRequest,
            headers: message.Headers);
        
        Debug.WriteLine(message.Headers.ToString());

        var guid = message.Headers.GetValues(nameof(randomizedRequest.GuidValue))
            .FirstOrDefault();
        guid.Should()
            .Be(string.Empty);
        var str = message.Headers.GetValues(nameof(randomizedRequest.StringValue))
            .FirstOrDefault();
        str.Should()
            .Be(string.Empty);

        var num = message.Headers.GetValues(nameof(randomizedRequest.NumericValue))
            .FirstOrDefault();
        num.Should()
            .Be(string.Empty);
        var arr = message.Headers.GetValues(nameof(randomizedRequest.StringArrayValue))
            .FirstOrDefault();
        arr.Should()
            .Be(string.Empty);
    }
}