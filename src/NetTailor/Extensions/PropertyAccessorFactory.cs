using System.Linq.Expressions;
using System.Reflection;

namespace NetTailor.Extensions;

internal static class PropertyAccessorFactory<T>
{
    private static readonly Dictionary<Type, List<PropertyAccessor<T>>> PropertyAccessorsCache = new();
    
    private static Type Type { get; } = typeof(T);
    private static PropertyInfo[] Properties { get; } = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    internal static List<PropertyAccessor<T>> GetPropertyAccessors()
    {
        if (!PropertyAccessorsCache.TryGetValue(Type, out var accessors))
        {
            accessors = new List<PropertyAccessor<T>>();

            foreach (var property in Properties)
            {
                if (property.CanRead)
                {
                    var getter = CompileGetter(property);
                    accessors.Add(new PropertyAccessor<T>(property.Name, getter));
                }
            }

            PropertyAccessorsCache[Type] = accessors;
        }

        return accessors;
    }
    
    private static Func<T, object> CompileGetter(PropertyInfo property)
    {
        var instance = Expression.Parameter(typeof(object), "instance");
        var castedInstance = Expression.Convert(instance, property.DeclaringType);
        var propertyAccess = Expression.Property(castedInstance, property);
        var result = Expression.Convert(propertyAccess, typeof(object));
        var lambda = Expression.Lambda<Func<T, object>>(result, instance);

        return lambda.Compile();
    }
}