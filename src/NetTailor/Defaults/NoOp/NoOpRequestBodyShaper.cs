using NetTailor.Abstractions;

namespace NetTailor.Defaults.NoOp;

internal class NoOpRequestBodyShaper<TRequest> : IRequestBodyShaper<TRequest>
{
    public static IRequestBodyShaper<TRequest> Instance { get; } = new NoOpRequestBodyShaper<TRequest>();
    public object? Shape(TRequest request) => null;
}