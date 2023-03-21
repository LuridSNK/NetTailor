using NetTailor.Abstractions;

namespace NetTailor.Defaults;

public class DefaultClientNameGetter<TRequest> : IClientNameGetter<TRequest>
{
    public DefaultClientNameGetter(string name) => Name = name;
    
    public string Name { get; }
}