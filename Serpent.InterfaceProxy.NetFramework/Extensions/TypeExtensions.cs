namespace Serpent.InterfaceProxy.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Serpent.InterfaceProxy.Types;

    public static class TypeExtensions
    {
        public static Type GetGenericSubstitute(this Type type, IReadOnlyDictionary<Type, Type> substitutes)
        {
            return TypeSubstitutor.GetSubstitutedType(type, substitutes);
        }

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

        public static Type MakeGenericTypeIfNecessary(this Type closureType, params Type[] parameterTypes)
        {
            if (parameterTypes.Length > 0)
            {
                return closureType.MakeGenericType(parameterTypes);
            }

            return closureType;
        }
    }
}