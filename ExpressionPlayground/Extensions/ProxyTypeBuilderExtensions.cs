namespace ExpressionPlayground.Extensions
{
    using System;
    using System.Reflection.Emit;

    public static class ProxyTypeBuilderExtensions
    {
        public static GeneratedProxy GenerateProxy<TInterfaceType>(this ProxyTypeBuilder proxyTypeBuilder)
        {
            return proxyTypeBuilder.GenerateProxy(typeof(TInterfaceType));
        }

        public static ProxyTypeBuilder ModuleBuilder(this ProxyTypeBuilder proxyTypeBuilder, Func<AssemblyBuilder, ModuleBuilder> selectBuilder)
        {
            proxyTypeBuilder.ModuleBuilder(selectBuilder(DefaultValues.DefaultAssemblyBuilder));
            return proxyTypeBuilder;
        }
    }
}