using System.Net.Http.Headers;
using NetTailor.Contracts;
using NetTailor.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetTailor.Example.Contracts;

namespace NetTailor.Example;

public sealed class ExampleClientProfile : IHttpServiceProfile
{
    private readonly IConfiguration _configuration;

    public ExampleClientProfile(IConfiguration configuration) => _configuration = configuration;

    public void Configure(IHttpServiceBuilder builder)
    {
        var uri = _configuration["SecondClient:Uri"]!;
        var scheme = _configuration["SecondClient:Scheme"]!;
        var token = _configuration["SecondClient:Token"]!;
        var client = builder.Create(client =>
            {
                client.BaseAddress = new Uri(uri);
                client.DefaultRequestHeaders.Add("Default", "Request-Header");
            })
            ;

        SampleRequests.SetupSimpleGet(client);
        SampleRequests.SetupSimplePost(client);
        SampleRequests.SetupSimpleDelete(client);
    }
}

public static class SampleRequests
{
    public static void SetupSimpleGet(IHttpClientBuilder builder)
    {
        builder.Get<SampleGet, SampleGetResponse>(
            r => $"sample/{r.Id}",
            requestBuilder =>
            {
                requestBuilder.Headers((_, h) => h.Add("Custom", "Request-Header"));
                requestBuilder.Query(q => new
                {
                    ThisContent = "WillBeSentInQueryString"
                });
            });
    }

    public static void SetupSimplePost(IHttpClientBuilder builder)
    {
        builder.Post<SimplePost, SimplePostResponse>("sample",
            requestBuilder =>
            {
                requestBuilder.Content(c => new { c.Items });
                requestBuilder.Headers((_, h) =>
                {
                    h.Authorization = new AuthenticationHeaderValue("Bearer", "12345");
                });
            });
    }

    public static void SetupSimpleDelete(IHttpClientBuilder builder)
    {
        builder.Delete<SimpleDelete, Empty>(r => $"sample/{r.Id}",
            _ => { });
    }
}