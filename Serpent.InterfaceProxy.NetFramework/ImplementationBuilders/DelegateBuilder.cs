namespace Serpent.InterfaceProxy.ImplementationBuilders
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    internal static class DelegateBuilder
    {
        /// <summary>
        ///     This creates the method that is called by the dynamically implemented method that calls
        /// </summary>
        /// <param name="delegateMethodName">The name of the delegate method to created</param>
        /// <param name="typeBuilder">The type builder in which the delegate is to be created</param>
        /// <param name="methodInfo">The method to be called when the delegate is executed</param>
        /// <param name="closureFinalType">The closure type</param>
        /// <param name="interfaceToImplement">The interface to implement</param>
        /// <returns>A metod builder</returns>
        public static MethodBuilder CreateDelegateMethod(
            string delegateMethodName,
            TypeBuilder typeBuilder,
            MethodInfo methodInfo,
            Type closureFinalType,
            Type interfaceToImplement)
        {
            var genericArguments = methodInfo.GetGenericArguments();

            var delegateParameters = ImmutableArray<Type>.Empty;

            if (closureFinalType != null)
            {
                delegateParameters = delegateParameters.Add(closureFinalType);
            }

            delegateParameters = delegateParameters.Add(interfaceToImplement);

            // Create delegate method - to be able to pass parameters
            var delegateMethodBuilder = typeBuilder.DefineMethod(
                delegateMethodName,
                MethodAttributes.Private,
                methodInfo.ReturnType,
                delegateParameters.ToArray());

            if (genericArguments.Length > 0)
            {
                delegateMethodBuilder.DefineGenericParameters(genericArguments.Select(ga => ga.Name).ToArray());
            }

            if (delegateParameters.Length == 1)
            {
                delegateMethodBuilder.DefineParameter(1, ParameterAttributes.None, "closureReference");
            }
            else
            {
                delegateMethodBuilder.DefineParameter(1, ParameterAttributes.None, "innerInterfaceReference");
                delegateMethodBuilder.DefineParameter(2, ParameterAttributes.None, "innerInterfaceReference");
            }

            // Get arguments from the closure type
            var closureFields = Array.Empty<FieldInfo>();

            if (closureFinalType != null)
            {
                closureFields = closureFinalType.GetFields();
            }

            var parameters = methodInfo.GetParameters();
            var generator = delegateMethodBuilder.GetILGenerator();

            return GenerateDelegateMethodIL(methodInfo, generator, parameters, closureFields, delegateMethodBuilder);
        }

        public static MethodBuilder GenerateDelegateMethodIL(
            MethodInfo methodInfo,
            ILGenerator generator,
            ParameterInfo[] parameters,
            FieldInfo[] closureFields,
            MethodBuilder delegateMethodBuilder)
        {
            if (parameters.Length == 0)
            {
                generator.Emit(OpCodes.Ldarg_1); // get the service parameter from the delegate
            }
            else
            {
                generator.Emit(OpCodes.Ldarg_2); // get the service parameter from the delegate
            }

            for (var i = 0; i < parameters.Length; i++)
            {
                generator.Emit(OpCodes.Ldarg_1); // the closure type parameter
                generator.Emit(OpCodes.Ldfld, closureFields[i]); // closure field i
            }

            generator.EmitCall(OpCodes.Callvirt, methodInfo, null); // call the same method on the .inner variable
            generator.Emit(OpCodes.Ret);

            return delegateMethodBuilder;
        }
    }
}
