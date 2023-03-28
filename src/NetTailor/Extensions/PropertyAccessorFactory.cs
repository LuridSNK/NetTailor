using System.Linq.Expressions;
using System.Reflection;

namespace NetTailor.Extensions;

internal static class PropertyAccessorFactory
{
    private static readonly Dictionary<Type, List<PropertyAccessor>> PropertyAccessorsCache = new();
    
    internal static List<PropertyAccessor> GetPropertyAccessors(object obj)
    {
        var type = obj.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        if (!PropertyAccessorsCache.TryGetValue(type, out var accessors))
        {
            accessors = new List<PropertyAccessor>();

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
    
    private static Func<object, object> CompileGetter(PropertyInfo property)
    {
        var instance = Expression.Parameter(typeof(object), "instance");
        var castedInstance = Expression.Convert(instance, property.DeclaringType);
        var propertyAccess = Expression.Property(castedInstance, property);
        var result = Expression.Convert(propertyAccess, typeof(object));
        var lambda = Expression.Lambda<Func<object, object>>(result, instance);

        return lambda.Compile();
    }
}

internal class PropertyAccessor
{
    public string PropertyName { get; }
    public Func<object, object> Getter { get; }

    public PropertyAccessor(string propertyName, Func<object, object> getter)
    {
        PropertyName = propertyName;
        Getter = getter;
    }
}