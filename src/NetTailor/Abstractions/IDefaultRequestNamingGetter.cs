using System.Text.Json;

namespace NetTailor.Abstractions;

public interface IDefaultRequestNamingGetter<TRequest>
{
    public JsonSerializerOptions JsonOptions { get;}
}