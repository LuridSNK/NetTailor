# NetTailor - Http weaved effortlessly

![CI](https://github.com/LuridSNK/NetTailor/workflows/CI/badge.svg)
[![NuGet](https://img.shields.io/nuget/dt/nettailor.svg)](https://www.nuget.org/packages/nettailor)
[![NuGet](https://img.shields.io/nuget/vpre/nettailor.svg)](https://www.nuget.org/packages/nettailor)

**NetTailor** simplifies development with a declarative, 
CQRS-like approach that reduces boilerplate code and streamlines maintenance, 
allowing you to focus on the data-in and data-out.

### Installing [NetTailor](https://www.nuget.org/packages/NetTailor)
With .NET CLI:
```bash
dotnet add package NetTailor
```
### Using NetTailor
#### - 1. Create your profile (or see an [example](https://github.com/LuridSNK/NetTailor/blob/master/src/Examples/NetTailor.Example/ExampleClientProfile.cs))
```csharp
// use against IServiceCollection
var clientBuilder = services.AddHttpClientProfile("example", client =>
{
    client.BaseAddress = new Uri("http://api.example.com/");
});
```
#### - 2. Create a request by chaining it with `IHttpClientBuilder`
```csharp

clientBuilder.Get<SampleGet, SampleGetResponse>(r => $"users/{r.Id}/friends", // configure route 
            reqBuilder => // configure request
            {
                reqBuilder.Headers((_, h) => h.Add("PER-REQUEST-HEADER", "some-value"));
                reqBuilder.Query(q => new { Sort = "Asc", OrderBy = nameof(q.Name) });
            });
// translates to GET http://api.example.com/sample/1?sort=Asc&orderBy=Name
```
#### - 3 Use it, by injecting `IRequestDispatcher` into your services
```csharp
var request = new SampleGet { Id = 1, Name = "John" };
var dispatcher = serviceProvider.GetService<IRequestDispatcher>();
var response = await dispatcher.Dispatch<SampleGet, SampleGetResponse>(request, CancellationToken.None);
```
#### - 4.1. Sending JSON
```csharp
clientBuilder.Post<SamplePost, SamplePostResponse>("users",
            reqBuilder => 
            {
                reqBuilder.Content(r => new 
                {
                    r.Email,
                    r.Name,
                    r.Password,
                    r.PasswordConfirmation,
                });
            });
```

#### - 4.2. Sending files
```csharp
clientBuilder.Post<SampleUpload, SampleUploadResult>(r => $"users/{r.Id}/photos"
            reqBuilder => 
            {
                reqBuilder.Form(r => new 
                {
                    Photo = r.File, // a stream, will be translated to form-data
                    r.Description,
                    r.IsPublic,
                });
            });
```
