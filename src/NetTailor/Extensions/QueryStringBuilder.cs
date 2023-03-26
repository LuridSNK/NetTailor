#nullable enable

using System.Collections;
using System.Text;
using System.Text.Json;

namespace NetTailor.Extensions;

public static class QueryStringBuilder
{
    private const string ErrorMessage =
        "An object can be either an anonymous type or concrete, but cannot be an instance of IConvertible";

    public static string Build<T>(StringBuilder stringBuilder, T? target, JsonNamingPolicy? policy = null)
    {
        if (target == null) return string.Empty;
        var type = typeof(T);
        if (IsUnsupported(type)) throw new InvalidOperationException(ErrorMessage);

        if (target is Dictionary<string, string> stringDict)
        {
            return BuildFromDictionary(stringBuilder, stringDict, policy);
        }
        
        if (target is Dictionary<string, IEnumerable<string>> stringCollectionDict)
        {
            return BuildFromDictionary(stringBuilder, stringCollectionDict, policy);
        }

        var accessors = PropertyAccessorFactory<T>.GetPropertyAccessors();
        var isFirstProperty = true;

        foreach (var accessor in accessors)
        {
            var value = accessor.Getter(target);
            if (value != null)
            {
                if (!isFirstProperty)
                {
                    stringBuilder.Append('&');
                }
                else
                {
                    stringBuilder.Append('?');
                    isFirstProperty = false;
                }

                if (value is IEnumerable collection and not string)
                {
                    var index = 0;
                    foreach (var item in collection)
                    {
                        if (index > 0)
                        {
                            stringBuilder.Append('&');
                        }

                        stringBuilder.Append(Uri.EscapeDataString(policy?.ConvertName(accessor.PropertyName) ?? accessor.PropertyName));
                        stringBuilder.Append('=');
                        stringBuilder.Append(Uri.EscapeDataString(item.ToString()!));
                        index++;
                    }
                }
                else
                {
                    stringBuilder.Append(Uri.EscapeDataString(policy?.ConvertName(accessor.PropertyName) ?? accessor.PropertyName));
                    stringBuilder.Append('=');
                    stringBuilder.Append(Uri.EscapeDataString(value.ToString()!));
                }
            }
        }

        return stringBuilder.ToString();
    }
    
    private static string BuildFromDictionary(StringBuilder sb, Dictionary<string, string> dict, JsonNamingPolicy? policy = null)
    {
        sb.Append('?');
        for (var i = 0; i < dict.Count; i++)
        {
            var kvp = dict.ElementAt(i);
            sb.AppendFormat("{0}={1}", policy?.ConvertName(kvp.Key) ?? kvp.Key, kvp.Value);
            if (i < dict.Count - 1) sb.Append('&');
        }

        return sb.ToString();
    }
    
    private static string BuildFromDictionary(StringBuilder sb, Dictionary<string, IEnumerable<string>> dict, JsonNamingPolicy? policy = null)
    {
        sb.Append('?');
        for (var i = 0; i < dict.Count; i++)
        {
            var kvp = dict.ElementAt(i);
            foreach (var val in kvp.Value)
            {
                sb.AppendFormat("{0}={1}", policy?.ConvertName(kvp.Key) ?? kvp.Key, val);
                sb.Append('&');
            }
        }

        sb.Remove(sb.Length - 1, 1);
        
        return sb.ToString();
    }
  
    private static readonly Type ConvertibleType = typeof(IConvertible);
    private static bool IsUnsupported(Type type) => ConvertibleType.IsAssignableFrom(type);
}

internal class PropertyAccessor<TObj>
{
    public string PropertyName { get; }
    public Func<TObj, object> Getter { get; }

    public PropertyAccessor(string propertyName, Func<TObj, object> getter)
    {
        PropertyName = propertyName;
        Getter = getter;
    }
}
