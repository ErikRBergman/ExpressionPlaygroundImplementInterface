namespace ExpressionPlayground
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    public struct GeneratedProxy
    {
        public GeneratedProxy(Type generatedType, IEnumerable<Type> interfacesImplemented, Delegate factory)
        {
            this.GeneratedType = generatedType;
            this.InterfacesImplemented = interfacesImplemented;
            this.Factory = factory;
        }

        public Type GeneratedType { get; }

        public IEnumerable<Type> InterfacesImplemented { get; }

        public Delegate Factory { get; }
    }
}