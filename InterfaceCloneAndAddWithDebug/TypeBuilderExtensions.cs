using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace InterfaceCloneAndAddWithDebug
{
    public static class TypeBuilderExtensions
    {
        public static TypeBuilder AddInterfaceImplementations(this TypeBuilder typeBuilder, IEnumerable<Type> types)
        {
            foreach (var type in types.Where(t => t.IsInterface))
            {
                typeBuilder.AddInterfaceImplementation(type);
            }

            return typeBuilder;
        }

        public static TypeBuilder AddInterfaceImplementations(this TypeBuilder typeBuilder, IEnumerable<Type> types, Func<Type, Type> substitueFunc)
        {
            foreach (var type in types.Where(t => t.IsInterface))
            {
                var newType = substitueFunc(type);
                typeBuilder.AddInterfaceImplementation(newType);
            }

            return typeBuilder;
        }
    }
}