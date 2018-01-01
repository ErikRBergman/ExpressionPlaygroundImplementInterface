namespace ExpressionPlayground
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    public struct GeneratedProxy
    {
        public GeneratedProxy(Type generatedType, IEnumerable<Type> interfacesImplemented)
        {
            this.GeneratedType = generatedType;
            this.InterfacesImplemented = interfacesImplemented;
        }

        public Type GeneratedType { get; }

        public IEnumerable<Type> InterfacesImplemented { get; }
    }
}