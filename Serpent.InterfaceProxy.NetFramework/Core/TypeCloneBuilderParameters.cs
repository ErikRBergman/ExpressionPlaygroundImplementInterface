namespace Serpent.InterfaceProxy.Core
{
    using System;
    using System.Collections.Immutable;
    using System.Reflection;
    using System.Reflection.Emit;

    using Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder;

    public class TypeCloneBuilderParameters : TypeCloneBuilderParameters<ProxyTypeBuilder.TypeContext, ProxyTypeBuilder.MethodContext>
    {
        public static new TypeCloneBuilderParameters New => new TypeCloneBuilderParameters();
    }

    public class TypeCloneBuilderParameters<TTypeContext, TMethodContext>
        where TTypeContext : BaseTypeContext<TTypeContext, TMethodContext>
        where TMethodContext : BaseMethodContext
    {
        public static TypeCloneBuilderParameters<TTypeContext, TMethodContext> New => new TypeCloneBuilderParameters<TTypeContext, TMethodContext>();

        public Func<CreateMethodData, TTypeContext, CreateMethodFuncResult<TMethodContext>> CreateMethodFunc { get; set; }

        /// <summary>
        ///     A predicate indicating whether to implement the interface the source type is implementing or not
        ///     The predicate is called for each source type interface.
        /// </summary>
        public Func<Type, bool> ImplementInterfacePredicate { get; set; }

        public ImmutableList<Type> InterfacesToImplement { get; set; } = ImmutableList<Type>.Empty;

        public MethodAttributes MethodAttributes { get; set; } =
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final;

        public ModuleBuilder ModuleBuilder { get; set; } = DefaultValues.DefaultModuleBuilder;

        public string Namespace { get; set; } = DefaultValues.DefaultTypeNamespace;

        public Action<TypeBuilder> OnBeforeTypeIsFinalizedAction { get; set; }

        public Action<TypeBuilder> OnTypeBuilderCreatedAction { get; set; }

        public Action<TypeBuilder> OnTypeBuilderCreatedAndConfiguredAction { get; set; }

        public Type ParentType { get; set; }

        public TypeAttributes TypeAttributes { get; set; } = TypeAttributes.Public;

        public string TypeName { get; set; }
    }
}