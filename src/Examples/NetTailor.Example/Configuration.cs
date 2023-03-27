using Microsoft.Extensions.Configuration;

namespace NetTailor.Example;

public static class Configuration
{
    public static IConfiguration Value => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            ["Api:Uri"] = "https://localhost:5001"
        }!)
        .Build();
}