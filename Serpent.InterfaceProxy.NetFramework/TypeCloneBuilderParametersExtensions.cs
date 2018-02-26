namespace Serpent.InterfaceProxy
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class TypeCloneBuilderParametersExtensions
    {
        public static TypeCloneBuilderParameters SetNewTypeIsInterface(this TypeCloneBuilderParameters parameters)
        {
            return parameters.NewTypeAttributes(TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public);
        }

        public static TypeCloneBuilderParameters NewTypeAttributes(this TypeCloneBuilderParameters parameters, TypeAttributes typeAttributes)
        {
            parameters.TypeAttributes = typeAttributes;
            return parameters;
        }

        public static TypeCloneBuilderParameters AddInterface(this TypeCloneBuilderParameters parameters, Type @interface)
        {
            parameters.InterfacesToImplement = parameters.InterfacesToImplement.Add(@interface);
            return parameters;
        }

        public static TypeCloneBuilderParameters AddInterface<TType>(this TypeCloneBuilderParameters parameters)
        {
            parameters.InterfacesToImplement = parameters.InterfacesToImplement.Add(typeof(TType));
            return parameters;
        }

        public static TypeCloneBuilderParameters ParentType(this TypeCloneBuilderParameters parameters, Type parentType)
        {
            parameters.ParentType = parentType;
            return parameters;
        }

        public static TypeCloneBuilderParameters ClosureTypeNameSelectorFunc(this TypeCloneBuilderParameters parameters, Func<Type, MethodInfo, string, string> closureTypeNameSelector)
        {
            parameters.ClosureTypeNameSelector = closureTypeNameSelector;
            return parameters;
        }

        public static TypeCloneBuilderParameters ImplementInterfacePredicateFunc(this TypeCloneBuilderParameters parameters, Func<Type, bool> predicate)
        {
            parameters.ImplementInterfacePredicate = predicate;
            return parameters;
        }

        public static TypeCloneBuilderParameters TypeName(this TypeCloneBuilderParameters parameters, string typeName)
        {
            parameters.TypeName = typeName;
            return parameters;
        }

        public static IsValidResult IsValid(this TypeCloneBuilderParameters parameters)
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

        public struct IsValidResult
        {
            public IsValidResult(bool isValid, IEnumerable<string> errors)
            {
                this.IsValid = isValid;
                this.Errors = errors;
            }

            public bool IsValid { get; }

            public IEnumerable<string> Errors { get; }
        }

    }
}