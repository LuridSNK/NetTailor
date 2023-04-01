using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.IO;
using NetTailor.Abstractions;
using NetTailor.Defaults;
using NetTailor.Defaults.ContentSerializers;
using NetTailor.Utilities;

namespace NetTailor.Extensions;

/// <summary>
/// Provides extensibility and configuration is CQRS style manner
/// </summary>
public static class HttpProfilesServiceCollectionExtensions
{
    /// <summary>
    /// Adds the IHttpClientFactory and related services to the IServiceCollection
    /// and configures a named HttpClient.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="name">The logical name of the <see cref="HttpClient"/> to configure.</param>
    /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    /// <exception cref="ArgumentNullException">When <see cref="IServiceCollection"/> is null.</exception>
    public static IHttpClientBuilder AddHttpClientProfile(
        this IServiceCollection services, 
        string name, 
        Action<HttpClient> configureClient)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.WriteLine("Name was not provided. Falling back to generated one.");
            return services.AddHttpClientProfile(configureClient);
        }
        
        Debug.WriteLine($"Registering a client with name '{name}'.");
        return services.AddUtilitiesHttpProfileDefaults()
            .AddHttpClient(name, configureClient);
    }
    
    /// <summary>
    /// Adds the IHttpClientFactory and related services to the IServiceCollection
    /// and configures HttpClient that will take a randomly generated name.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configureClient">A delegate that is used to configure an <see cref="HttpClient"/>.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    /// <exception cref="ArgumentNullException">When <see cref="IServiceCollection"/> is null.</exception>
    public static IHttpClientBuilder AddHttpClientProfile(
        this IServiceCollection services,
        Action<HttpClient> configureClient)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        var name = RandomNames.Generate();
        Debug.WriteLine($"Registering a client with generated name '{name}'.");
        return services.AddUtilitiesHttpProfileDefaults()
            .AddHttpClient(name, configureClient);
    }
    
    public static IServiceCollection AddHttpClientProfile<TProfile>(this IServiceCollection services) 
        where TProfile : class, IHttpServiceProfile
    {
        var desc = new ServiceDescriptor(typeof(IHttpServiceProfile), typeof(TProfile), ServiceLifetime.Transient);
        services.Add(desc);
        var sp = services.BuildServiceProvider();
        var service = sp.GetRequiredService<IHttpServiceProfile>();
        service.Configure(new DefaultHttpServiceBuilder(services));
        services.Remove(desc);
        return services;
    }
    
    private static IServiceCollection AddUtilitiesHttpProfileDefaults(this IServiceCollection services)
    {        
        Debug.WriteLine("Registering required utility dependencies...");
        services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.TryAddSingleton<ObjectPool<StringBuilder>>(sp =>
        {
            var provider = sp.GetRequiredService<ObjectPoolProvider>();
            var policy = new StringBuilderPooledObjectPolicy();
            return provider.Create(policy);
        });
        services.TryAddSingleton<CamelCase>();
        services.TryAddSingleton<RecyclableMemoryStreamManager>();
        services.TryAddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();
        services.TryAddSingleton<IRequestExecutionContextFactory, RequestExecutionContextFactory>();
        services.TryAddSingleton<IRequestDispatcher, DefaultRequestDispatcher>();
        services.TryAddSingleton<IContentWriterReaderFactory, ContentContentWriterReaderFactory>();

        Debug.WriteLine("All required utility dependencies has been registered!");
        return services;
    }
}
