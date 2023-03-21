using System.Net;
using System.Text.Json;
using NetTailor.Example;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetTailor;
using NetTailor.Extensions;

// define service collection & configuration
var services = new ServiceCollection();
IConfiguration configuration = new ConfigurationManager().AddInMemoryCollection(new KeyValuePair<string, string?>[]
{
    new("SecondClient:Uri", "https://api.sample-client-service-2.com"),
    new("SecondClient:Scheme", "Bearer"),
    new("SecondClient:Token", "jwt_ForSecondClient.236fd5e1f2074e9f8d24a3d3e1ea78bc.fake"),
    
}).Build();
services.AddSingleton(configuration);
// add typed httpProfile
services.AddHttpClientProfile<SampleClientProfileTwo>();

// add httpProfile fluently
services.AddHttpClientProfile("SampleClientProfile-1", client =>
    {
        client.BaseAddress = new Uri("https://sample-client-service-1.com/api");
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddDefaultHeaders(headers => headers.Add("header_from_defaults", Guid.NewGuid().ToString("N")))
    .Post<ServiceOneRequest, ServiceOneResponse>(
        "some-static-route",
        builder =>
    {
        builder.UseNaming(Naming.UpperSnakeCase);
        builder.Query(q => new { q.Name });
        builder.Content(c => new { c.Id, c.CreatedAt }, Naming.LowerSnakeCase);
        builder.Headers((_, headers) => headers
            .Authorization("Bearer", "jwt.236fd5e1f2074e9f8d24a3d3e1ea78bc.fake")
            .Add("request-header-1", "value-1")
            .Add("request-header-2", "value-2"));
    })
    .AddHttpMessageHandler(_ => new MockingDelegatingHandler(new ServiceOneResponse(Executed: true), HttpStatusCode.OK));

// build service provider & get http dispatcher
var serviceProvider = services.BuildServiceProvider();
var dispatcher = serviceProvider.GetRequiredService<IHttpDispatcher>();

// send requests
var serviceOneCmd = new ServiceOneRequest(Guid.NewGuid(), "Kirll_Runk", DateTimeOffset.UtcNow);
var serviceOneResult = await dispatcher.Dispatch<ServiceOneRequest, ServiceOneResponse>(serviceOneCmd, CancellationToken.None);
Console.ForegroundColor = ConsoleColor.Green;
await Console.Out.WriteLineAsync(serviceOneResult.Successful
    ? $"RESPONSE SUCCESSFUL:\n{JsonSerializer.Serialize(serviceOneResult.Value, JsonSerializerOptionsCache.GetSettingsOrDefault(Naming.LowerSnakeCase))}"
    : $"FAILED: {serviceOneResult.Exception}");
Console.ResetColor();

await Console.Out.WriteLineAsync("\n\n");

var serviceTwoCmd = new ServiceTwoRequest(777);
var serviceTwoResult = await dispatcher.Dispatch<ServiceTwoRequest, ServiceTwoResponse>(serviceTwoCmd, CancellationToken.None);
Console.ForegroundColor = ConsoleColor.Green;
await Console.Out.WriteLineAsync(serviceOneResult.Successful
    ? $"RESPONSE SUCCESSFUL:\n{JsonSerializer.Serialize(serviceTwoResult.Value, JsonSerializerOptionsCache.GetSettingsOrDefault(Naming.UpperSnakeCase))}"
    : $"FAILED: {serviceTwoResult.Exception}");
Console.ResetColor();

await Console.Out.WriteLineAsync("\n\n");

var serviceTwoEmptyCmd = new ServiceTwoEmptyRequest();
var serviceTwoEmptyResult = await dispatcher.Dispatch(serviceTwoEmptyCmd, CancellationToken.None);
Console.ForegroundColor = ConsoleColor.Green;
await Console.Out.WriteLineAsync(serviceTwoEmptyResult.Successful
    ? $"RESPONSE SUCCESSFUL:\n{serviceTwoEmptyResult.Value}"
    : $"FAILED: {serviceTwoResult.Exception}");
Console.ResetColor();


// define contracts for fluent client
public record ServiceOneRequest(Guid Id, string Name, DateTimeOffset CreatedAt) : IHttpRequest<ServiceOneResponse>;
public record ServiceOneResponse(bool Executed);