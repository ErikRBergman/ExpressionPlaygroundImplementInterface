﻿// ReSharper disable StyleCop.SA1402
// ReSharper disable ParameterHidesMember

namespace Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading.Tasks;

    using Serpent.InterfaceProxy.Extensions;
    using Serpent.InterfaceProxy.ImplementationBuilders;

    public class ProxyTypeBuilder : TypeCloneBuilder<ProxyTypeBuilder.TypeContext, ProxyTypeBuilder.MethodContext>
    {
        public override GenerateTypeResult GenerateType(TypeCloneBuilderParameters<TypeContext, MethodContext> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            parameters.CreateMethodFunc = this.CreateMethodContextData;
            return base.GenerateType(parameters);
        }

        protected override void EmitMethodImplementation(Type interfaceType, MethodInfo sourceMethodInfo, MethodBuilder methodBuilder, MethodContext context, Type parentType)
        {
            var closureFinalType = context.ClosureFinalType;

            var generator = methodBuilder.GetILGenerator();
            var parameters = sourceMethodInfo.GetParameters();

            var hasParameters = parameters.Length != 0;

            var isAsyncMethod = sourceMethodInfo.ReturnType.Is<Task>();

            var returnValueGenericArguments = sourceMethodInfo.ReturnType.GenericTypeArguments;
            var hasGenericReturnValueArguments = returnValueGenericArguments.Length != 0;

            var hasAsyncReturnValue = isAsyncMethod && returnValueGenericArguments.Length == 1;
            var hasNonAsyncReturnValue = !isAsyncMethod && typeof(void) != sourceMethodInfo.ReturnType;
            var hasReturnValue = hasAsyncReturnValue || hasNonAsyncReturnValue;
            var returnsVoid = sourceMethodInfo.ReturnType == typeof(void);

            var returnType = sourceMethodInfo.ReturnType;

            if (hasReturnValue && isAsyncMethod)
            {
                returnType = sourceMethodInfo.ReturnType.GenericTypeArguments[0];
            }

#if DEBUG
            var methodName = sourceMethodInfo.Name;
            var _debug_test_ = methodName;
#endif


            var proxyMethod = GetProxyMethod(closureFinalType, parentType, hasParameters, hasReturnValue, returnType, hasGenericReturnValueArguments, isAsyncMethod);

            var proxyMethodParameters = proxyMethod.GetProxyMethodParameters();

            // Start of call to the proxy method (being called in the end of this method). Moved ldarg_0 here to optimize
            generator.Emit(OpCodes.Ldarg_0); // this (from the current method)

            foreach (var parameter in proxyMethodParameters)
            {

                if (parameter.ProxyMethodParameterType == ProxyMethodParameterType.ParametersClosure)
                {
                    // only emit instantiating the closure if it's needed
                    if (hasParameters)
                    {
                        var closureConstructor = closureFinalType.GetConstructors().Single();

                        generator.Emit(OpCodes.Newobj, closureConstructor);

                        // Populate the closure
                        var closureFields = closureFinalType.GetFields();

                        var parameterCount = parameters.Length;
                        for (var i = 0; i < parameterCount; i++)
                        {
                            generator.Emit(OpCodes.Dup);

                            generator.LdArg(i + 1);

                            generator.Emit(OpCodes.Stfld, closureFields[i]);
                        }
                    }
                    else
                    {
                        throw new Exception($"ProxyMethodParameter \"{parameter.ParameterInfo.Name}\" is specified with a closure parameter, even though the method does not have any parameters.");
                    }
                }

                if (parameter.ProxyMethodParameterType == ProxyMethodParameterType.MethodDelegate)
                {

                    // Second parameter to the Func<*> constructor  
                    // get the address of this.DelegateMethodAsync
                    generator.Emit(OpCodes.Ldarg_0); // this.
                    generator.Emit(OpCodes.Ldftn, context.FinalDelegateMethodInfo); // the delegate method

                    // Instantiate the Func<*>
                    var delegateType = GetDelegateType(interfaceType, sourceMethodInfo, closureFinalType, hasParameters, returnsVoid);

                    var delegateConstructor = delegateType.GetConstructors().Single(p => p.GetParameters().Length == 2);
                    generator.Emit(OpCodes.Newobj, delegateConstructor);
                }
            }

            generator.EmitCall(OpCodes.Call, proxyMethod, null); // call the proxy method
            generator.Emit(OpCodes.Ret);
        }

        private static Type GetDelegateType(Type interfaceType, MethodInfo sourceMethodInfo, Type closureFinalType, bool hasParameters, bool returnsVoid)
        {
            Type delegateType;
            if (hasParameters)
            {
                if (!returnsVoid)
                {
                    delegateType = typeof(Func<,,>).MakeGenericType(closureFinalType, interfaceType, sourceMethodInfo.ReturnType);
                }
                else
                {
                    delegateType = typeof(Action<,>).MakeGenericType(closureFinalType, interfaceType);
                }
            }
            else
            {
                if (!returnsVoid)
                {
                    delegateType = typeof(Func<,>).MakeGenericType(interfaceType, sourceMethodInfo.ReturnType);
                }
                else
                {
                    delegateType = typeof(Action<>).MakeGenericType(interfaceType);
                }
            }

            return delegateType;
        }

        private static Type GetFinalClosureType(
            Type @interface,
            Func<Type, ModuleBuilder, MethodInfo, TypeBuilder> createClosureTypeBuilderFunc,
            ModuleBuilder moduleBuilder,
            ParameterInfo[] parameters,
            MethodInfo method,
            Type[] genericArguments)
        {
            Type closureFinalType = null;

            if (parameters.Length > 0)
            {
                var closureTypeBuilder = createClosureTypeBuilderFunc(@interface, moduleBuilder, method);
                closureFinalType = ClosureBuilder.CreateClosureType(closureTypeBuilder, method).MakeGenericTypeIfNecessary(genericArguments);
            }

            return closureFinalType;
        }

        private static MethodInfo GetFinalDelegateMethod(TypeBuilder typeBuilder, Type @interface, MethodInfo method, Type closureFinalType, Type[] genericArguments)
        {
            var delegateMethodName = method.Name + "_delegate_" + Guid.NewGuid().ToString("N");
            var delegateMethodBuilder = DelegateBuilder.CreateDelegateMethod(delegateMethodName, typeBuilder, method, closureFinalType, @interface);
            return delegateMethodBuilder.MakeGenericMethodIfNecessary(genericArguments);
        }

        private static MethodInfo GetProxyMethod(
            Type finalClosureType,
            Type parentType,
            bool hasParameters,
            bool hasReturnValue,
            Type returnType,
            bool hasGenericReturnValueArguments,
            bool isAsyncMethod)
        {
            var proxyMethods = parentType.GetInstanceMethods().Where(im => im.GetCustomAttributes(typeof(ProxyMethodAttribute), true).Any());

            var proxyMethodGenericParameters = ImmutableArray<Type>.Empty;

            if (hasParameters)
            {
                proxyMethodGenericParameters = proxyMethodGenericParameters.Add(finalClosureType);
            }

            if (hasReturnValue)
            {
                proxyMethodGenericParameters = proxyMethodGenericParameters.Add(returnType);

                if (hasGenericReturnValueArguments)
                {
                    proxyMethods = proxyMethods.Where(method => method.ReturnType.ContainsGenericParameters);
                }

                proxyMethods = proxyMethods.Where(method => method.ReturnType != typeof(void) && method.ReturnType != typeof(Task));
            }
            else
            {
                proxyMethods = proxyMethods.Where(method => method.ReturnType == typeof(void) || method.ReturnType == typeof(Task));
            }

            // get Task<> return type if needed
            proxyMethods = proxyMethods.Where(method => method.ReturnType.Is<Task>() == isAsyncMethod);

            var minimumParameterCount = 1 + (hasParameters ? 1 : 0);

            proxyMethods = proxyMethods
                .Select(m => new { Method = m, Parameters = m.GetParameters() })
                .Where(method => method.Parameters.Length >= minimumParameterCount)
                .Where(method =>
                    {
                        if (hasParameters == true)
                        {
                            return method.Parameters.Count(
                                    p => p.GetCustomAttribute<ProxyMethodParameterTypeAttribute>()?.ParameterType == ProxyMethodParameterType.ParametersClosure) == 1;
                        }

                        return method.Parameters.All(
                            p => p.GetCustomAttribute<ProxyMethodParameterTypeAttribute>()?.ParameterType != ProxyMethodParameterType.ParametersClosure);
                    })
                .OrderByDescending(method => method.Parameters.Length).Select(method => method.Method);

            var proxyMethod = proxyMethods.FirstOrDefault();

            if (proxyMethod == null)
            {
                throw new Exception("Not a single method matching the proxy method required could be found");
            }

            if (proxyMethodGenericParameters.Length > 0)
            {
                proxyMethod = proxyMethod.MakeGenericMethod(proxyMethodGenericParameters.ToArray());
            }

            return proxyMethod;
        }

        private TypeBuilder CreateClosureTypeFunc(Type interfaceType, ModuleBuilder moduleBuilder, MethodInfo method)
        {
            var closureTypeName = method.Name + "_closure_" + Guid.NewGuid().ToString("N"); // this.ClosureTypeNameSelector(@interfaceType, method, this.Namespace);
            return ClosureBuilder.CreateClosureTypeBuilder(moduleBuilder, closureTypeName);
        }

        private CreateMethodFuncResult<MethodContext> CreateMethodContextData(CreateMethodData methodData, TypeContext typeContext)
        {
            var finalClosureType = GetFinalClosureType(
                methodData.SourceType,
                this.CreateClosureTypeFunc,
                typeContext.Parameters.ModuleBuilder,
                methodData.SourceMethodInfo.GetParameters(),
                methodData.SourceMethodInfo,
                methodData.GenericArguments);
            var finalDelegateMethod = GetFinalDelegateMethod(
                methodData.TypeBuilder,
                methodData.SourceType,
                methodData.SourceMethodInfo,
                finalClosureType,
                methodData.GenericArguments);

            return new CreateMethodFuncResult<MethodContext>(
                methodData,
                new MethodContext
                {
                    ClosureFinalType = finalClosureType,
                    FinalDelegateMethodInfo = finalDelegateMethod
                });
        }

        public class MethodContext : BaseMethodContext
        {
            public Type ClosureFinalType { get; set; }

            public MethodInfo FinalDelegateMethodInfo { get; set; }
        }

        public class TypeContext : BaseTypeContext<TypeContext, MethodContext>
        {
        }
    }
}