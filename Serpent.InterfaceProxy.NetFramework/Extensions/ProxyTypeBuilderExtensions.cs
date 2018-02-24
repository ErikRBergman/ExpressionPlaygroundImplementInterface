namespace Serpent.InterfaceProxy.Extensions
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    using Serpent.InterfaceProxy;

    public static class ProxyTypeBuilderExtensions
    {
        public static GenerateProxyResult<TInterfaceType> GenerateProxy<TInterfaceType>(this ProxyTypeBuilder proxyTypeBuilder)
        {
            var generatedType = proxyTypeBuilder.GenerateTypeClone(new GenerateTypeParameters(typeof(TInterfaceType)));

            return new GenerateProxyResult<TInterfaceType>(generatedType.GeneratedType, generatedType.InterfacesImplemented, (Func<TInterfaceType, TInterfaceType>)generatedType.Factory);
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