namespace Serpent.InterfaceProxy.ImplementationBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    internal static class DefaultConstructorGenerator
    {
        public static void CreateDefaultConstructors(TypeBuilder typeBuilder, Type parentType)
        {
            var baseTypeConstructors = parentType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var baseConstructor in baseTypeConstructors)
            {
                CreateDefaultConstructor(typeBuilder, baseConstructor);
            }
        }

        private static void CreateDefaultConstructor(TypeBuilder typeBuilder, ConstructorInfo baseConstructor)
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

        private static void GenerateParentTypeConstructorCall(ILGenerator generator, IReadOnlyCollection<ParameterInfo> parameters, ConstructorInfo baseConstructor)
        {
            generator.Emit(OpCodes.Ldarg_0);
            for (var i = 1; i <= parameters.Count; ++i)
            {
                generator.Emit(OpCodes.Ldarg, i);
            }

            generator.Emit(OpCodes.Call, baseConstructor);

            generator.Emit(OpCodes.Ret);
        }
    }
}