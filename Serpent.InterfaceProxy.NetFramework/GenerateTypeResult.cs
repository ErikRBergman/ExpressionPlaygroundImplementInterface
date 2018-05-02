namespace Serpent.InterfaceProxy
{
    using System;
    using System.Collections.Generic;

    public struct GenerateTypeResult
    {
        public GenerateTypeResult(Type generatedType, IEnumerable<Type> interfacesImplemented, IReadOnlyDictionary<Type, Delegate> factories)
        {
            this.GeneratedType = generatedType;
            this.InterfacesImplemented = interfacesImplemented;
            this.Factories = factories;
        }

        public Type GeneratedType { get; }

        public IEnumerable<Type> InterfacesImplemented { get; }

        public IReadOnlyDictionary<Type, Delegate> Factories { get; }
    }

    public struct GenerateProxyResult<TInterface>
    {
        public GenerateProxyResult(Type generatedType, IEnumerable<Type> interfacesImplemented, Func<TInterface, TInterface> factory)
        {
            this.GeneratedType = generatedType;
            this.InterfacesImplemented = interfacesImplemented;
            this.Factory = factory;
        }

        public Type GeneratedType { get; }

        public IEnumerable<Type> InterfacesImplemented { get; }

        public Func<TInterface, TInterface> Factory { get; }
    }
}