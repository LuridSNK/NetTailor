namespace NetTailor.Abstractions;

public interface IFormBuilder<TRequest>
{
    ValueTask<HttpContent?> Build(TRequest request);
}