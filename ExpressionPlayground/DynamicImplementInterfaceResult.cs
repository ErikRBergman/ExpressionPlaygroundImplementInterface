namespace ExpressionPlayground
{
    using System;
    using System.Collections.Immutable;

    public struct DynamicImplementInterfaceResult
    {
        public DynamicImplementInterfaceResult(ImmutableList<Type> interfacesImplemented, ImmutableList<string> namesUsed)
        {
            this.InterfacesImplemented = interfacesImplemented;
            this.NamesUsed = namesUsed;
        }

        public ImmutableList<Type> InterfacesImplemented { get; }

        public ImmutableList<string> NamesUsed { get; }

        public static DynamicImplementInterfaceResult Empty => new DynamicImplementInterfaceResult(ImmutableList<Type>.Empty, ImmutableList<string>.Empty);
    }
}