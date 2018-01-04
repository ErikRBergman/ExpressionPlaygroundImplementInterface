namespace ExpressionPlayground.Methods
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading.Tasks;

    using Sigil.NonGeneric;

    internal static class MethodBuilder
    {
        public static void EmitMethodImplementation(
                    Type interfaceType,
                    MethodInfo sourceMethodInfo,
                    TypeBuilder typeBuilder,
                    Type[] genericArguments,
                    Type closureFinalType,
                    MethodInfo finalDelegateMethod,
                    Type parentType)
        {
            var parameters = sourceMethodInfo.GetParameters();

            var emit = Emit.BuildInstanceMethod(
                sourceMethodInfo.ReturnType,
                parameters.Select(p => p.ParameterType).ToArray(),
                typeBuilder,
                sourceMethodInfo.Name,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final,
                false, false
                );

            var hasParameters = parameters.Length != 0;
            var hasReturnValue = sourceMethodInfo.ReturnType.GenericTypeArguments.Length == 1;

            // only emit instantiating the closure if it's needed
            if (hasParameters)
            {
                var closureVariable = emit.DeclareLocal(closureFinalType, "closure");
                emit.NewObject(closureFinalType);

                // Populate the closure
                var closureFields = closureFinalType.GetFields();

                for (ushort i = 1; i < parameters.Length + 1; i++)
                {
                    emit.Duplicate();
                    emit.LoadArgument(i);
                    emit.StoreField(closureFields[i - 1]);
                }

                // Store the closure in our local variable
                emit.StoreLocal(closureVariable);
            }

            // Start of call to this.ExecuteAsync
            emit.LoadArgument(0); // this (from the current method)

            // With no parameters, the closure does not need to be pushed either
            if (hasParameters)
            {
                // First parameter to the Func<*> constructor
                emit.LoadLocal("closure");
            }

            // Second parameter to the Func<*> constructor
            // get the address of this.DelegateMethodAsync
            emit.LoadArgument(0);

            emit.LoadFunctionPointer(finalDelegateMethod);

            // Instantiate the Func<*>
            Type delegateType;

            ConstructorInfo delegateConstructor;

            if (hasParameters)
            {
                delegateType = typeof(Func<,,>).MakeGenericType(closureFinalType, interfaceType, sourceMethodInfo.ReturnType);
                delegateConstructor = delegateType.GetConstructors().Single(p => p.GetParameters().Length == 2);
            }
            else
            {
                delegateType = typeof(Func<,>).MakeGenericType(interfaceType, sourceMethodInfo.ReturnType);
                delegateConstructor = delegateType.GetConstructors().Single(p => p.GetParameters().Length == 2);
            }

            emit.NewObject(delegateConstructor);

            var executeAsyncMethods = ProxyTypeBuilder.GetExecuteAsyncMethods(parentType);

            MethodInfo executeAsync = null;

            var executeAsyncGenericParameters = ImmutableArray<Type>.Empty;

            if (hasParameters)
            {
                executeAsyncGenericParameters = executeAsyncGenericParameters.Add(closureFinalType);
            }

            if (hasReturnValue)
            {
                executeAsyncGenericParameters = executeAsyncGenericParameters.Add(sourceMethodInfo.ReturnType.GenericTypeArguments[0]);
                executeAsyncMethods = executeAsyncMethods.Where(method => method.ReturnType.ContainsGenericParameters);
            }
            else
            {
                executeAsyncMethods = executeAsyncMethods.Where(method => !method.ReturnType.ContainsGenericParameters);
            }

            var executeAsyncParameterCount = 1 + (hasParameters ? 1 : 0);

            executeAsync = executeAsyncMethods.Single(method => method.GetParameters().Length == executeAsyncParameterCount);

            if (executeAsyncGenericParameters.Length > 0)
            {
                executeAsync = executeAsync.MakeGenericMethod(executeAsyncGenericParameters.ToArray());
            }

            emit.Call(executeAsync);
            emit.Return();
        }
    }
}