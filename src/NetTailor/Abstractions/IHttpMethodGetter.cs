namespace NetTailor.Abstractions;

public interface IHttpMethodGetter<in TRequest>
{
    HttpMethod Method { get; }
}