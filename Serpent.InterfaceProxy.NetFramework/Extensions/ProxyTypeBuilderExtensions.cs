namespace Serpent.InterfaceProxy.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Serpent.InterfaceProxy;

    public static class ProxyTypeBuilderExtensions
    {
        public static GenerateProxyResult<TInterfaceType> GenerateProxy<TInterfaceType>(this ProxyTypeBuilder proxyTypeBuilder, Type parentType)
        {
            var parameters = TypeCloneBuilderParameters
                .New
                .AddInterface(typeof(TInterfaceType))
                .TypeName(typeof(TInterfaceType).FullName)
                .ParentType(parentType);

            var generatedType = proxyTypeBuilder.GenerateType(parameters);

            return new GenerateProxyResult<TInterfaceType>(generatedType.GeneratedType, generatedType.InterfacesImplemented, (Func<TInterfaceType, TInterfaceType>)generatedType.Factories.FirstOrDefault(f => f is Func<TInterfaceType, TInterfaceType>));
        }


    }
}