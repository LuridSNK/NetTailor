#nullable enable

using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace NetTailor.Extensions;

public static class QueryStringBuilder
{
    private static readonly Dictionary<Type, List<PropertyAccessor>> PropertyAccessorsCache = new();

    private const string ErrorMessage =
        "An object can be either an anonymous type or concrete, but cannot be an instance of IConvertible";

    public static string Build(StringBuilder queryStringBuilder, object? target, JsonNamingPolicy? policy = null)
    {
        if (target == null) return string.Empty;
        var type = target.GetType();

        if (IsUnsupported(type)) throw new InvalidOperationException(ErrorMessage);

        var accessors = GetPropertyAccessors(type);
        var isFirstProperty = true;

        foreach (var accessor in accessors)
        {
            var value = accessor.Getter(target);
            if (value != null)
            {
                if (!isFirstProperty)
                {
                    queryStringBuilder.Append('&');
                }
                else
                {
                    queryStringBuilder.Append('?');
                    isFirstProperty = false;
                }

                if (value is IEnumerable collection and not string)
                {
                    var index = 0;
                    foreach (var item in collection)
                    {
                        if (index > 0)
                        {
                            queryStringBuilder.Append('&');
                        }

                        queryStringBuilder.Append(Uri.EscapeDataString(policy?.ConvertName(accessor.PropertyName) ?? accessor.PropertyName));
                        queryStringBuilder.Append('=');
                        queryStringBuilder.Append(Uri.EscapeDataString(item.ToString()!));
                        index++;
                    }
                }
                else
                {
                    queryStringBuilder.Append(Uri.EscapeDataString(policy?.ConvertName(accessor.PropertyName) ?? accessor.PropertyName));
                    queryStringBuilder.Append('=');
                    queryStringBuilder.Append(Uri.EscapeDataString(value.ToString()!));
                }
            }
        }

        return queryStringBuilder.ToString();
    }

    private static List<PropertyAccessor> GetPropertyAccessors(Type type)
    {
        if (!PropertyAccessorsCache.TryGetValue(type, out var accessors))
        {
            accessors = new List<PropertyAccessor>();

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.CanRead)
                {
                    var getter = CompileGetter(property);
                    accessors.Add(new PropertyAccessor(property.Name, getter));
                }
            }

            PropertyAccessorsCache[type] = accessors;
        }

        return accessors;
    }


    private static readonly Type ConvertibleType = typeof(IConvertible);
    
    private static bool IsUnsupported(Type type) => 
        ConvertibleType.IsAssignableFrom(type);
    

    private const string ExpressionParameter = "instance";

    private static Func<object, object> CompileGetter(PropertyInfo property)
    {
        var instance = Expression.Parameter(typeof(object), ExpressionParameter);
        var castedInstance = Expression.Convert(instance, property.DeclaringType);
        var propertyAccess = Expression.Property(castedInstance, property);
        var result = Expression.Convert(propertyAccess, typeof(object));
        var lambda = Expression.Lambda<Func<object, object>>(result, instance);

        return lambda.Compile();
    }

    private class PropertyAccessor
    {
        public string PropertyName { get; }
        public Func<object, object> Getter { get; }

        public PropertyAccessor(string propertyName, Func<object, object> getter)
        {
            PropertyName = propertyName;
            Getter = getter;
        }
    }
}