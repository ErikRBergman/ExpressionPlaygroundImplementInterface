namespace ExpressionPlayground.Closures
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    internal static class ClosureBuilder
    {
        public static Type CreateClosureType(TypeBuilder closureTypeBuilder, MethodInfo sourceMethodInfo)
        {

            var parameters = sourceMethodInfo.GetParameters();

            var genericArguments = sourceMethodInfo.GetGenericArguments();
            var closureGenericArguments = genericArguments.Length == 0
                                              ? Array.Empty<GenericTypeParameterBuilder>()
                                              : closureTypeBuilder.DefineGenericParameters(genericArguments.Select(ga => ga.Name).ToArray());

            // Create all parameters from the source method into the closure type
            foreach (var parameter in parameters)
            {
                if (sourceMethodInfo.Name == "GenericsAndVarArgs")
                {
                }

                var parameterType = parameter.ParameterType;

                if (parameterType.BaseType == typeof(System.Array))
                {
                    var elementType = parameterType.GetElementType();
                    var arrayRank = parameterType.GetArrayRank();
                    elementType = GetSubstitutedType(genericArguments, elementType, closureGenericArguments);

                    if (arrayRank != 1)
                    {
                        parameterType = elementType.MakeArrayType(arrayRank);
                    }
                    else
                    {
                        parameterType = elementType.MakeArrayType();
                    }
                }
                else
                {
                    parameterType = GetSubstitutedType(genericArguments, parameterType, closureGenericArguments);
                }


                closureTypeBuilder.DefineField(parameter.Name, parameterType, FieldAttributes.Public);
            }

            closureTypeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.HideBySig);
            var closureType = closureTypeBuilder.CreateType();

            return closureType;
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

        public static TypeBuilder CreateClosureTypeBuilder(ModuleBuilder moduleBuilder, string closureTypeName)
        {
            ////return moduleBuilder.DefineType(closureTypeName, TypeAttributes.BeforeFieldInit | TypeAttributes.SequentialLayout | TypeAttributes.Public | TypeAttributes.Sealed, typeof(ValueType));
            return moduleBuilder.DefineType(closureTypeName, TypeAttributes.BeforeFieldInit | TypeAttributes.Public | TypeAttributes.Sealed);
        }

    }
}