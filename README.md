# HttTailor
#### Weave your web requests with a bit of tailor's touch
A library for fluent HttpClient configuration and CQRS-like usage.

### Usage

Add [nuget](https://www.nuget.org/packages/HttTailor/) package or `dotnet add package HttTailor` in dotnet CLI.

Add a reference to `HttTailor.DependencyInjection` and call the provided extension method in your ConfigureServices method to configure your interface:
```csharp
// provide a marker, where HttpProfile implementations are located
services.AddHttpProfiles<MyAssemblyMarker>(); 
```

Define a service profile:
```csharp
public class GithubProfile : IHttpServiceProfile
{
    // any additional dependencies can also be injected
    private readonly IConfiguration _configuration;
    
    public GithubProfile(IConfiguration configuration) =>
        _configuration = configuration;
    
    public void Configure(IHttpServiceBuilder builder)
    {     
        // Define service variables
        builder
            // base url
            .WithBaseUrl("https://api.github.com/")   
            // define property naming per service, no need to use JsonPropertyAttribute     
            .WithServiceWideNamingPolicy(NamingPolicies.LowerSnakeCase);
            // add IAsyncPolicy from Polly for service (can be multiple, if wrapping)
            .WithRequestPolicy(myPolicy)
            // add DelegatingHandler for service (can be multiple)
            .WithHttpMessageHandler(myHandler);
            
            
        // Then configure HTTP requests:
        builder.Get<GetUserRequest, GetUserResponse>(
            // subroute configuration
            request => $"users/{request.userName}",
            message => message.
                .Query(req => new { QueryValues = req.ItemsForQuery })
                .Headers((req, headers) => 
                {
                    headers.Add("Accept", "application/vnd.github+json"); 
                })
        );
        
        var bearerToken = _configuration.GetValue<string>("GithubApi:Token");
        builder.Post<RenameBranch, RenameBranchResponse>(
            request => $"repos/{request.Owner}/{request.Repo}/branches/{request.Branch}/rename",
            message => message.
                // since we've defined a naming policy in WithServiceWideNamingPolicy(NamingPolicies.LowerSnakeCase)
                // we don't need to bother about namings anymore
                .Content(req => new { req.NewName })
                .Headers((req, headers) => 
                {
                    headers.Add("Accept", "application/vnd.github+json"); 
                    headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                });
        );
    }
}
```