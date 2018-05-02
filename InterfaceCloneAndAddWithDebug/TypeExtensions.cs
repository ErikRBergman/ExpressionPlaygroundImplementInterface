namespace InterfaceCloneAndAddWithDebug
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using InterfaceCloneAndAddWithDebug.Interfaces;

    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetActorInterfaces(this Type type)
        {
            return type.GetInterfaces().Where(i => typeof(IActor).IsAssignableFrom(i) && i.GetInterfaces().Contains(typeof(IActor)));
        }

        public static IEnumerable<Type> GetActorInterfaces(this Type type, bool includeMe)
        {
            var interfaces = type.GetActorInterfaces();
            return includeMe ? new[] { type }.Union(interfaces) : interfaces;
        }
    }
}