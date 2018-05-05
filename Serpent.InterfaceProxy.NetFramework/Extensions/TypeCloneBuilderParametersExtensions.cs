namespace Serpent.InterfaceProxy.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Serpent.InterfaceProxy.Core;

    public static class TypeCloneBuilderParametersExtensions
    {
        public static TypeCloneBuilderParameters<TTypeContext, TMethodContext> AddInterface<TTypeContext, TMethodContext>(
            this TypeCloneBuilderParameters<TTypeContext, TMethodContext> parameters,
            Type @interface)
            where TTypeContext : BaseTypeContext<TTypeContext, TMethodContext>
            where TMethodContext : BaseMethodContext
        {
            parameters.InterfacesToImplement = parameters.InterfacesToImplement.Add(@interface);
            return parameters;
        }

        public static TypeCloneBuilderParameters<TTypeContext, TMethodContext> AddInterface<TTypeContext, TMethodContext, TType>(
            this TypeCloneBuilderParameters<TTypeContext, TMethodContext> parameters)
            where TTypeContext : BaseTypeContext<TTypeContext, TMethodContext>
            where TMethodContext : BaseMethodContext
        {
            parameters.InterfacesToImplement = parameters.InterfacesToImplement.Add(typeof(TType));
            return parameters;
        }

        public static TypeCloneBuilderParameters<TTypeContext, TMethodContext> ImplementInterfacePredicateFunc<TTypeContext, TMethodContext>(
            this TypeCloneBuilderParameters<TTypeContext, TMethodContext> parameters,
            Func<Type, bool> predicate)
            where TTypeContext : BaseTypeContext<TTypeContext, TMethodContext>
            where TMethodContext : BaseMethodContext
        {
            parameters.ImplementInterfacePredicate = predicate;
            return parameters;
        }

        public static IsValidResult IsValid<TTypeContext, TMethodContext>(this TypeCloneBuilderParameters<TTypeContext, TMethodContext> parameters)
            where TTypeContext : BaseTypeContext<TTypeContext, TMethodContext>
            where TMethodContext : BaseMethodContext
        {
            var errors = new List<string>();

            if (parameters.InterfacesToImplement.IsEmpty)
            {
                errors.Add("No interfaces to implement");
            }

            if (parameters.TypeName == null)
            {
                errors.Add("TypeName must not be null");
            }

            foreach (var type in parameters.InterfacesToImplement)
            {
                if (type.IsInterface == false)
                {
                    errors.Add(type.FullName + " is not an interface");
                }
            }

            if (parameters.ParentType != null)
            {
                if (parameters.ParentType.ContainsGenericParameters)
                {
                    errors.Add("Parent type has generic arguments. No support for creating generic types at the moment");
                }
            }

            return new IsValidResult(errors.Count == 0, errors);
        }

        public static TypeCloneBuilderParameters<TTypeContext, TMethodContext> MethodAttributes<TTypeContext, TMethodContext>(
            this TypeCloneBuilderParameters<TTypeContext, TMethodContext> parameters,
            MethodAttributes methodAttributes)
            where TTypeContext : BaseTypeContext<TTypeContext, TMethodContext>
            where TMethodContext : BaseMethodContext
        {
            parameters.MethodAttributes = methodAttributes;
            return parameters;
        }

        public static TypeCloneBuilderParameters<TTypeContext, TMethodContext> ModuleBuilder<TTypeContext, TMethodContext>(
            this TypeCloneBuilderParameters<TTypeContext, TMethodContext> parameters,
            ModuleBuilder moduleBuilder)
            where TTypeContext : BaseTypeContext<TTypeContext, TMethodContext>
            where TMethodContext : BaseMethodContext
        {
            parameters.ModuleBuilder = moduleBuilder;
            return parameters;
        }

        public static TypeCloneBuilderParameters<TTypeContext, TMethodContext> OutputInterface<TTypeContext, TMethodContext>(
            this TypeCloneBuilderParameters<TTypeContext, TMethodContext> parameters)
            where TTypeContext : BaseTypeContext<TTypeContext, TMethodContext>
            where TMethodContext : BaseMethodContext
        {
            return parameters.TypeAttributes(System.Reflection.TypeAttributes.Interface | System.Reflection.TypeAttributes.Abstract | System.Reflection.TypeAttributes.Public)
                .MethodAttributes(
                    System.Reflection.MethodAttributes.Public
                    | System.Reflection.MethodAttributes.Virtual
                    | System.Reflection.MethodAttributes.HideBySig
                    | System.Reflection.MethodAttributes.NewSlot
                    | System.Reflection.MethodAttributes.Abstract);
        }

        public static TypeCloneBuilderParameters<TTypeContext, TMethodContext> ParentType<TTypeContext, TMethodContext>(
            this TypeCloneBuilderParameters<TTypeContext, TMethodContext> parameters,
            Type parentType)
            where TTypeContext : BaseTypeContext<TTypeContext, TMethodContext>
            where TMethodContext : BaseMethodContext
        {
            parameters.ParentType = parentType;
            return parameters;
        }

        public static TypeCloneBuilderParameters<TTypeContext, TMethodContext> TypeAttributes<TTypeContext, TMethodContext>(
            this TypeCloneBuilderParameters<TTypeContext, TMethodContext> parameters,
            TypeAttributes typeAttributes)
            where TTypeContext : BaseTypeContext<TTypeContext, TMethodContext>
            where TMethodContext : BaseMethodContext
        {
            parameters.TypeAttributes = typeAttributes;
            return parameters;
        }

        public static TypeCloneBuilderParameters<TTypeContext, TMethodContext> TypeName<TTypeContext, TMethodContext>(
            this TypeCloneBuilderParameters<TTypeContext, TMethodContext> parameters,
            string typeName)
            where TTypeContext : BaseTypeContext<TTypeContext, TMethodContext>
            where TMethodContext : BaseMethodContext
        {
            parameters.TypeName = typeName;
            return parameters;
        }

        public struct IsValidResult
        {
            public IsValidResult(bool isValid, IEnumerable<string> errors)
            {
                this.IsValid = isValid;
                this.Errors = errors;
            }

            public IEnumerable<string> Errors { get; }

            public bool IsValid { get; }
        }
    }
}