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

            var genericArgumentArray = sourceMethodInfo.GetGenericArguments();
            var closureGenericArguments = genericArgumentArray.Length == 0
                                              ? Array.Empty<GenericTypeParameterBuilder>()
                                              : closureTypeBuilder.DefineGenericParameters(genericArgumentArray.Select(ga => ga.Name).ToArray());

            // Create all parameters from the source method into the closure type
            foreach (var parameter in parameters)
            {
                var parameterType = parameter.ParameterType;

                var index = Array.IndexOf(genericArgumentArray, parameterType);
                if (index != -1)
                {
                    parameterType = closureGenericArguments[index];
                }

                closureTypeBuilder.DefineField(parameter.Name, parameterType, FieldAttributes.Public);
            }

            closureTypeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            var closureType = closureTypeBuilder.CreateType();

            return closureType;
        }

        public static TypeBuilder CreateClosureTypeBuilder(ModuleBuilder moduleBuilder, string closureTypeName)
        {
            ////return moduleBuilder.DefineType(closureTypeName, TypeAttributes.BeforeFieldInit | TypeAttributes.SequentialLayout | TypeAttributes.Public | TypeAttributes.Sealed, typeof(ValueType));
            return moduleBuilder.DefineType(closureTypeName, TypeAttributes.BeforeFieldInit | TypeAttributes.SequentialLayout | TypeAttributes.Public | TypeAttributes.Sealed);
        }

    }
}