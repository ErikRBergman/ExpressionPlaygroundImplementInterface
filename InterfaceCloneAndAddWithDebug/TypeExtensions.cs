namespace InterfaceCloneAndAddWithDebug
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using InterfaceCloneAndAddWithDebug.Interfaces;

    using Serpent.InterfaceProxy.Extensions;

    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetActorInterfaces(this Type type)
        {
            // EB REVIEW: Doesnt both conditions produce the same result?
            return type.GetInterfaces().Where(i => i.Is<IActor>() && i.GetInterfaces().Contains(typeof(IActor)));
        }

        public static IEnumerable<Type> GetActorInterfaces(this Type type, bool includeMe)
        {
            var interfaces = type.GetActorInterfaces();
            return includeMe ? new[] { type }.Union(interfaces) : interfaces;
        }
    }
}