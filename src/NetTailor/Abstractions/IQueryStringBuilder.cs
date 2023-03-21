namespace NetTailor.Abstractions;

public interface IQueryStringBuilder<in TRequest>
{
    public ValueTask<string> Build(TRequest request);
}