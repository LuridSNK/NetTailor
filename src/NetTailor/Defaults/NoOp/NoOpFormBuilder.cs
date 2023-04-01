using NetTailor.Abstractions;

namespace NetTailor.Defaults.NoOp;

public class NoOpFormBuilder<TRequest> : IFormBuilder<TRequest>
{
    public ValueTask<HttpContent?> Build(TRequest request)
    {
        return new ValueTask<HttpContent?>((HttpContent)null);
    }

    public static IFormBuilder<TRequest> Instance { get; } = new NoOpFormBuilder<TRequest>();
}