namespace ExpressionPlayground.Constructor
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    internal static class DefaultConstructorGenerator
    {
        public static void CreateSuperClassConstructorCalls(TypeBuilder typeBuilder, Type parentType)
        {
            var baseTypeConstructors = parentType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var baseConstructor in baseTypeConstructors)
            {
                var parameters = baseConstructor.GetParameters();
                if (parameters.Length > 0 && parameters.Last().IsDefined(typeof(ParamArrayAttribute), false))
                {
                    throw new InvalidOperationException("Variadic constructors are not supported");
                }

                var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();

                var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, baseConstructor.CallingConvention, parameterTypes);
                for (var i = 0; i < parameters.Length; ++i)
                {
                    var parameter = parameters[i];

                    var parameterBuilder = ctor.DefineParameter(i + 1, parameter.Attributes, parameter.Name);

                    if (parameter.Attributes.HasFlag(ParameterAttributes.HasDefault))
                    {
                        parameterBuilder.SetConstant(parameter.RawDefaultValue);
                    }
                }

                var getIL = ctor.GetILGenerator();
                GenerateParentTypeConstructorCall(getIL, parameters, baseConstructor);
            }
        }

        private static void GenerateParentTypeConstructorCall(ILGenerator getIL, ParameterInfo[] parameters, ConstructorInfo baseConstructor)
        {
            getIL.Emit(OpCodes.Ldarg_0);
            for (var i = 1; i <= parameters.Length; ++i)
            {
                getIL.Emit(OpCodes.Ldarg, i);
            }

            getIL.Emit(OpCodes.Call, baseConstructor);

            getIL.Emit(OpCodes.Ret);
        }
    }
}