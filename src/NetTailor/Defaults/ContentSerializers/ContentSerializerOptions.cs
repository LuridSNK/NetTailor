using Microsoft.Extensions.DependencyInjection;
using NetTailor.Abstractions;

namespace NetTailor.Defaults.ContentSerializers;

public class ContentSerializerOptions
{
    public Func<IServiceProvider, IContentReader> ContentReader { get; internal set; } = 
        sp => sp.GetRequiredService<CamelCase>();

    public Func<IServiceProvider, IContentWriter> ContentWriter { get; internal set; } =
        sp => sp.GetRequiredService<CamelCase>();

}