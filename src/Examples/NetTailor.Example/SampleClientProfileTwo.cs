using System.Net;
using NetTailor.Contracts;
using NetTailor.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NetTailor.Example;

public sealed class SampleClientProfileTwo : IHttpServiceProfile
{
    private readonly IConfiguration _configuration;

    public SampleClientProfileTwo(IConfiguration configuration) => _configuration = configuration;

    public void Configure(IHttpServiceBuilder builder)
    {
        var uri = _configuration["SecondClient:Uri"]!;
        var scheme = _configuration["SecondClient:Scheme"]!;
        var token = _configuration["SecondClient:Token"]!;
        builder.Create(client =>
            {
                client.BaseAddress = new Uri(uri);
            })
            .AddDefaultHeaders(headers => headers
                .Authorization(scheme, token))
            .Get<ServiceTwoRequest, ServiceTwoResponse>(r => $"samples/{r.Id}", requestBuilder =>
            {
                requestBuilder.Query(q => new { Venom = "Snake", Ocelot = new[] {"la", "le", "lu", "le", "lo"} }, Naming.LowerSnakeCase);
                requestBuilder.Headers((_, headers) => headers
                    .Add("from-request-for-profile-2", new []{ "some-values", "1", "2" }));
            })
            .Post<ServiceTwoEmptyRequest, Empty>("to-empty", _ =>
            {
            })
            .AddHttpMessageHandler(_ => new MockingDelegatingHandler(new ServiceTwoResponse
            {
                Name = "John",
                Surname = "Doe",
                Age = 33,
                DateOfBirth = DateTimeOffset.UtcNow.Date.AddYears(-33).AddDays(-Random.Shared.Next(1, 90)),
                Tags = new[] { "fishing", "coding", "shooting", "bikes" }
            }, HttpStatusCode.OK, 
                Naming.CamelCase))
            ;
    }
}

public record ServiceTwoRequest(long Id) : IHttpRequest<ServiceTwoResponse>;

public class ServiceTwoResponse
{
    public string Name { get; init; } = null!;
    public string Surname { get; init; } = null!;
    public int Age { get; init; }
    public DateTime DateOfBirth { get; init; }
    public string[] Tags { get; init; } = null!;
}

public record ServiceTwoEmptyRequest : IHttpRequest<Empty>;