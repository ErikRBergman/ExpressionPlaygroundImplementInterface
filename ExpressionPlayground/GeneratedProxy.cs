namespace ExpressionPlayground
{
    using System;
    using System.Reflection.Emit;

    public struct GeneratedProxy
    {
        public GeneratedProxy(AssemblyBuilder assemblyBuilder, Type generatedType)
        {
            this.AssemblyBuilder = assemblyBuilder;
            this.GeneratedType = generatedType;
        }

        public AssemblyBuilder AssemblyBuilder { get; }

        public Type GeneratedType { get; }
    }
}