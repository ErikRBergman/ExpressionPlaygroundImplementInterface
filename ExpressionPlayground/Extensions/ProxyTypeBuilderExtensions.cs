namespace ExpressionPlayground.Extensions
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    public static class ProxyTypeBuilderExtensions
    {
        public static GeneratedProxy<TInterfaceType> GenerateProxy<TInterfaceType>(this ProxyTypeBuilder proxyTypeBuilder)
        {
            var generatedType =  proxyTypeBuilder.GenerateProxy(typeof(TInterfaceType));

            return new GeneratedProxy<TInterfaceType>(generatedType.GeneratedType, generatedType.InterfacesImplemented, (Func<TInterfaceType, TInterfaceType>)generatedType.Factory);
        }

        public static ProxyTypeBuilder ModuleBuilder(this ProxyTypeBuilder proxyTypeBuilder, Func<AssemblyBuilder, ModuleBuilder> selectBuilder)
        {
            proxyTypeBuilder.ModuleBuilder(selectBuilder(DefaultValues.DefaultAssemblyBuilder));
            return proxyTypeBuilder;
        }

        public static ProxyTypeBuilder Namespace(this ProxyTypeBuilder proxyTypeBuilder, string @namespace)
        {
            proxyTypeBuilder.Namespace = @namespace;
            return proxyTypeBuilder;
        }

        public static ProxyTypeBuilder ClosureTypeNameSelector(this ProxyTypeBuilder proxyTypeBuilder, Func<Type, MethodInfo, string, string> closureTypeNameSelector)
        {
            proxyTypeBuilder.ClosureTypeNameSelector = closureTypeNameSelector ?? throw new ArgumentNullException(nameof(closureTypeNameSelector));
            return proxyTypeBuilder;
        }

        public static ProxyTypeBuilder ModuleBuilder(this ProxyTypeBuilder proxyTypeBuilder, ModuleBuilder moduleBuilder)
        {
            proxyTypeBuilder.ModuleBuilder = moduleBuilder;
            return proxyTypeBuilder;
        }

        public static ProxyTypeBuilder ProxyTypeNameSelector(this ProxyTypeBuilder proxyTypeBuilder, Func<Type, string, string> proxyTypeNameSelectorFunc)
        {
            proxyTypeBuilder.ProxyTypeNameSelectorFunc = proxyTypeNameSelectorFunc;
            return proxyTypeBuilder;
        }

    }
}