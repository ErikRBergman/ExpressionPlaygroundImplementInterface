﻿namespace Serpent.InterfaceProxy.Extensions
{
    using System;
    using System.Linq;

    using Serpent.InterfaceProxy.Core;
    using Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder;

    public static class ProxyTypeBuilderExtensions
    {
        public static GenerateProxyResult<TInterfaceType> GenerateProxy<TInterfaceType>(this ProxyTypeBuilder proxyTypeBuilder, Type parentType, string typeName = null)
        {
            var parameters = TypeCloneBuilderParameters<ProxyTypeBuilder.TypeContext, ProxyTypeBuilder.MethodContext>.New.AddInterface(typeof(TInterfaceType))
                .TypeName(typeName ?? typeof(TInterfaceType).FullName + "Proxy")
                .ParentType(parentType);

            var generatedType = proxyTypeBuilder.GenerateType(parameters);

            return new GenerateProxyResult<TInterfaceType>(
                generatedType.GeneratedType,
                generatedType.InterfacesImplemented,
                (Func<TInterfaceType, TInterfaceType>)generatedType.Factories.FirstOrDefault(f => f.GetType().Is<TInterfaceType>()));
        }
    }
}