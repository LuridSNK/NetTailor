using HttTailor.Abstractions;

namespace HttTailor.Defaults;

public class DefaultClientNameGetter<TRequest> : IClientNameGetter<TRequest>
{
    public DefaultClientNameGetter(string name) => Name = name;
    
    public string Name { get; }
}