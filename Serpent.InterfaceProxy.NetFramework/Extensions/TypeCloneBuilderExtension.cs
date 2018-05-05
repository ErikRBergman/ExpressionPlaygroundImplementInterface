namespace Serpent.InterfaceProxy.Extensions
{
    using System;

    using Serpent.InterfaceProxy.Core;

    public static class TypeCloneBuilderExtension
    {
        public static GenerateTypeResult GenerateType<TTypeContext, TMethodContext>(
            this TypeCloneBuilder<TTypeContext, TMethodContext> proxyTypeBuilder,
            Action<TypeCloneBuilderParameters<TTypeContext, TMethodContext>> parametersFunc)
            where TTypeContext : BaseTypeContext<TTypeContext, TMethodContext>, new()
            where TMethodContext : BaseMethodContext, new()
        {
            var parameters = TypeCloneBuilderParameters<TTypeContext, TMethodContext>.New;
            parametersFunc(parameters);
            return proxyTypeBuilder.GenerateType(parameters);
        }
    }
}