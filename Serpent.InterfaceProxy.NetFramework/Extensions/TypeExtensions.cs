namespace Serpent.InterfaceProxy.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;

    using Serpent.InterfaceProxy.Types;

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

        public static Type GetGenericSubstitute(this Type type, IReadOnlyDictionary<Type, Type> substitutes)
        {
            return TypeSubstitutor.GetSubstitutedType(type, substitutes);
        }

        public static IEnumerable<MethodInfo> GetInstanceMethods(this Type type, params string[] methodNames)
        {
            if (methodNames == null || methodNames.Length == 0)
            {
                return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }

            return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(method => methodNames.Any(mn => string.CompareOrdinal(method.Name, mn) == 0));
        }

        /// <summary>
        ///     Determines whether an instance of a type implements or inherits from another type.
        ///     For example: The Melon type inherits from Fruit (which implements IFruit).
        ///     typeof(Melon).Is(typeof(Fruit) returns true.
        ///     typeof(Fruit).Is(typeof(Melon) returns false.
        ///     typeof(Melon).Is(typeof(IFruit)) returns true.
        /// </summary>
        /// <param name="type">The type to check if it inherits from or implements</param>
        /// <param name="otherType">The type to check if its implemented or is a base class of type</param>
        /// <returns>True or false</returns>
        public static bool Is(this Type type, Type otherType)
        {
            return otherType.IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether an instance of a type implements or inherits from another type.
        ///     For example: The Melon type inherits from Fruit (which implements IFruit).
        ///     typeof(Melon).Is(typeof(Fruit) returns true.
        ///     typeof(Fruit).Is(typeof(Melon) returns false.
        ///     typeof(Melon).Is(typeof(IFruit)) returns true.
        /// </summary>
        /// <typeparam name="TType">The type to check for
        /// </typeparam>
        /// <param name="type">
        /// The type to check if it inherits from or implements
        /// </param>
        /// <returns>
        /// True or false
        /// </returns>
        public static bool Is<TType>(this Type type)
        {
            return type.Is(typeof(TType));
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