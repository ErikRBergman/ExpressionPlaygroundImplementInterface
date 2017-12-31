namespace ExpressionPlayground
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

        public ImmutableList<Type> InterfacesImplemented { get; }

        public ImmutableList<string> NamesUsed { get; }

        public static ImplementInterfaceMethodResult Empty => new ImplementInterfaceMethodResult(ImmutableList<Type>.Empty, ImmutableList<string>.Empty);
    }
}