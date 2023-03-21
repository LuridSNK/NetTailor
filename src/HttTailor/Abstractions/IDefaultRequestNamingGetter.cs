using System.Text.Json;

namespace HttTailor.Abstractions;

public interface IDefaultRequestNamingGetter<TRequest>
{
    public JsonSerializerOptions JsonOptions { get;}
}