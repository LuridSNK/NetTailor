namespace NetTailor.Abstractions;

public interface IClientNameGetter<TRequest>
{
    public string Name { get; }
}