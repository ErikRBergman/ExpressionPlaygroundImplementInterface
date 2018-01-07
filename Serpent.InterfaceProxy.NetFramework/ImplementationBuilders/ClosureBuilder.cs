namespace Serpent.InterfaceProxy.ImplementationBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Serpent.InterfaceProxy.Extensions;
    using Serpent.InterfaceProxy.Types;

    internal static class ClosureBuilder
    {
        public static Type CreateClosureType(TypeBuilder closureTypeBuilder, MethodInfo sourceMethodInfo)
        {
            var parameters = sourceMethodInfo.GetParameters();

            var genericArguments = sourceMethodInfo.GetGenericArguments();
            var closureGenericArguments = genericArguments.Length == 0
                                              ? Array.Empty<GenericTypeParameterBuilder>()
                                              : closureTypeBuilder.DefineGenericParameters(genericArguments.Select(ga => ga.Name).ToArray());

            ////var substituteTypes = genericArguments.Zip(closureGenericArguments, (a, b) => new KeyValuePair<Type, Type>(a, b)).ToDictionary(p => p.Key, p => p.Value);
            var substituteTypes = genericArguments.ZipToDictionary(closureGenericArguments);

            // Create all parameters from the source method into the closure type
            foreach (var parameter in parameters)
            {
                var parameterType = parameter.ParameterType;

                parameterType = TypeSubstitutor.GetSubstitutedType(parameterType, substituteTypes);
                closureTypeBuilder.DefineField(parameter.Name, parameterType, FieldAttributes.Public);
            }

            closureTypeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.HideBySig);
            var closureType = closureTypeBuilder.CreateTypeInfo();
            return closureType;
        }

        public static TypeBuilder CreateClosureTypeBuilder(ModuleBuilder moduleBuilder, string closureTypeName)
        {
            ////return moduleBuilder.DefineType(closureTypeName, TypeAttributes.BeforeFieldInit | TypeAttributes.SequentialLayout | TypeAttributes.Public | TypeAttributes.Sealed, typeof(ValueType));
            return moduleBuilder.DefineType(closureTypeName, TypeAttributes.BeforeFieldInit | TypeAttributes.Public | TypeAttributes.Sealed);
        }

        private static Type GetSubstitutedType(Type[] genericArgumentArray, Type parameterType, GenericTypeParameterBuilder[] closureGenericArguments)
        {
            var index = Array.IndexOf(genericArgumentArray, parameterType);
            if (index != -1)
            {
                parameterType = closureGenericArguments[index];
            }

            return parameterType;
        }
    }
}