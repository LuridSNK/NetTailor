namespace HttTailor.Abstractions;

public interface IHttpMethodGetter<in TRequest>
{
    HttpMethod Method { get; }
}