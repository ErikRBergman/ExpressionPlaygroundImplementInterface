namespace ExpressionPlayground.Extensions
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;

    public static class TypeExtensions
    {
        public static ImmutableHashSet<Type> GetAllInterfaces(this Type interfaceType)
        {
            var interfaces = ImmutableHashSet<Type>.Empty;
            if (interfaceType.IsInterface)
            {
                interfaces = interfaces.Add(interfaceType);
            }

            return GetAllInterfaces(interfaceType, interfaces);
        }

        public static ImmutableHashSet<Type> GetAllInterfaces(this Type interfaceType, ImmutableHashSet<Type> interfaces)
        {
            return interfaceType.GetInterfaces().Aggregate(interfaces, (current, i) => current.Add(i));
        }
    }
}