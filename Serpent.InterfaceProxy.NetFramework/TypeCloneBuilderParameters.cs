namespace Serpent.InterfaceProxy
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Reflection;
    using System.Reflection.Emit;

    public class TypeCloneBuilderParameters
    {
        public static TypeCloneBuilderParameters New => new TypeCloneBuilderParameters();

        /// <summary>
        ///     A predicate indicating whether to implement the interface the source type is implementing or not
        ///     The predicate is called for each source type interface.
        /// </summary>
        public Func<Type, bool> ImplementInterfacePredicate { get; set; }

        public ImmutableList<Type> InterfacesToImplement { get; set; } = ImmutableList<Type>.Empty;

        public TypeAttributes TypeAttributes { get; set; } = TypeAttributes.Public;

        public Type ParentType { get; set; }

        public string TypeName { get; set; }

        public MethodAttributes MethodAttributes { get; set; } =
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final;

        public Func<Type, MethodInfo, string, string> ClosureTypeNameSelector { get; set; } = (@interface, methodInfo, @namespace) =>
            @namespace + "." + @interface.Name + "." + methodInfo.Name + "_Closure_" + Guid.NewGuid().ToString("N");

        public string Namespace { get; set; } = DefaultValues.DefaultTypeNamespace;

        public ModuleBuilder ModuleBuilder { get; set; }

        public Func<MethodInfo, Type, IEnumerable<TypeBuilderMethodParameter>, IEnumerable<TypeBuilderMethodParameter>> MethodModifierFunc { get; set; }
    }
}