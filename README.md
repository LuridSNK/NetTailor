# NetTailor
### Weave your network like a master craftsman.
A library for fluent HttpClient configuration and CQRS-like usage.

### Setting Up NetTailor

Add [nuget](https://www.nuget.org/packages/NetTailor/) package or `dotnet add package NetTailor` in dotnet CLI.

## Defining profiles:

There are two ways of setting up your HttpProfiles:

1. Register it using fluent interfaces by calling `AddHttpClientProfile()` on `IServiceCollection`:

```csharp
// define client
services.AddHttpClientProfile("MyFirstHttpService", client =>
    {
        client.BaseAddress = new Uri("https://example-website.com");
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    // configure default request headers if necessary
    .AddDefaultHeaders(headers => headers.Add("my_default_header", "default_header_value"))
    // configure your endpoint and message values
    .Post<ServiceOneRequest, ServiceOneResponse>(
        request => $"endpoint/{request.Id}",
        builder =>
    {
        // set naming policy
        builder.UseNaming(Naming.UpperSnakeCase);
        // configure query, content and per-request headers
        builder.Query(q => new { q.Name });
        builder.Content(c => new { c.Id, c.CreatedAt }, Naming.LowerSnakeCase);
        builder.Headers((_, headers) => headers            
            .Add("per_reqest_header", "per_request_header_value")
            .Add("another_one", "value_two"));
    })
```

2. Define your service by implementing `IHttpServiceProfile` interface, and register it using fluent interfaces by calling `AddHttpClientProfile<TProfile>();` on `IServiceCollection`:

```csharp
public sealed class MySecondHttpService : IHttpServiceProfile
{
    // convinient Dependency Injection
    private readonly IConfiguration _configuration;

    public SampleClientProfileTwo(IConfiguration configuration) => 
        _configuration = configuration;

    public void Configure(IHttpServiceBuilder builder)
    {
        var uri = _configuration.GetValue<string>("SecondService:Uri");
        var scheme = _configuration.GetValue<string>("SecondService:Scheme");
        var token = _configuration.GetValue<string>("SecondService:Token");
        
        // create a client definition
        builder.Create(client =>
            {
                client.BaseAddress = new Uri(uri);
            })
            // add default headers
            .AddDefaultHeaders(headers => headers
                .Authorization(scheme, token))
            // define your requests and values
            .Get<ServiceTwoRequest, ServiceTwoResponse>(r => $"endpoint/{r.Id}", requestBuilder =>
            {
                requestBuilder.Query(q => new { Venom = "Snake", OcelotSays = new[] {"la", "le", "lu", "le", "lo"} }, Naming.LowerSnakeCase);
                requestBuilder.Headers((_, headers) => headers
                    .Add("set-of-values", new []{ "v1", "v2", "v3" }));
            });
    }
}
```
## Calling your services in a CQRS-like fashion:
To call your requests you simply need to call for `IHttpDispatcher` via Dependency Injection:
```csharp
// example
var dispatcher = serviceProvider.GetRequiredService<IHttpDispatcher>();
var command = new YourCommand(/* your object*/)
await dispatcher.Dispatch<YourCommand, YourResponse>(command, CancellationToken.None);
```