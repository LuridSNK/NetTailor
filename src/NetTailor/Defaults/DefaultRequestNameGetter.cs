using NetTailor.Abstractions;

namespace NetTailor.Defaults;

public class DefaultRequestNameGetter<TRequest> : IRequestNameGetter<TRequest>
{
    public DefaultRequestNameGetter(string name) => Name = name;
    
    public string Name { get; }
}