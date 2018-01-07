namespace Serpent.InterfaceProxy
{
    using System;
    using System.Collections.Immutable;

    public struct ImplementInterfaceMethodResult
    {
        public ImplementInterfaceMethodResult(ImmutableList<Type> interfacesImplemented, ImmutableList<string> namesUsed)
        {
            this.InterfacesImplemented = interfacesImplemented;
            this.NamesUsed = namesUsed;
        }

        public static ImplementInterfaceMethodResult Empty => new ImplementInterfaceMethodResult(ImmutableList<Type>.Empty, ImmutableList<string>.Empty);

        public ImmutableList<Type> InterfacesImplemented { get; }

        public ImmutableList<string> NamesUsed { get; }
    }
}