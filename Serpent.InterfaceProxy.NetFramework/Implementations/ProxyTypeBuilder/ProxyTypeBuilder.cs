// ReSharper disable StyleCop.SA1402
// ReSharper disable ParameterHidesMember

namespace Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading.Tasks;

    using Serpent.InterfaceProxy.Core;
    using Serpent.InterfaceProxy.Extensions;
    using Serpent.InterfaceProxy.ImplementationBuilders;

    public class ProxyTypeBuilder : TypeCloneBuilder<ProxyTypeBuilder.TypeContext, ProxyTypeBuilder.MethodContext>
    {
        private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<MethodInfo>> TypeMethodInfoLookup = new ConcurrentDictionary<Type, IReadOnlyCollection<MethodInfo>>();

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

            var proxyMethod = GetProxyMethod(closureFinalType, parentType, hasParameters, hasReturnValue, returnType, hasGenericReturnValueArguments, isAsyncMethod, interfaceType);

            var proxyMethodParameters = proxyMethod.GetProxyMethodParameters();

            // Start of call to the proxy method (being called in the end of this method). Moved ldarg_0 here to optimize
            generator.Emit(OpCodes.Ldarg_0); // this (from the current method)

            foreach (var parameter in proxyMethodParameters)
            {
                switch (parameter.ProxyMethodParameterType)
                {
                    case ProxyMethodParameterType.TypeName:
                        EmitTypeName(interfaceType, parameter, generator);
                        break;

                    case ProxyMethodParameterType.MethodName:
                        EmitMethodName(sourceMethodInfo, parameter, generator);
                        break;

                    case ProxyMethodParameterType.ParametersClosure:
                        EmitParametersClosure(parameter, closureFinalType, generator, parameters);
                        break;

                    case ProxyMethodParameterType.MethodDelegate:
                        EmitMethodDelegate(interfaceType, sourceMethodInfo, context, generator, closureFinalType, hasParameters, returnsVoid);
                        break;

                    default:
                        throw new Exception($"Could not determine the parameter type of proxy method parameter \"{parameter.ParameterInfo.Name}\" .");
                }
            }

            generator.EmitCall(OpCodes.Call, proxyMethod, null); // call the proxy method
            generator.Emit(OpCodes.Ret);
        }

        private static void EmitMethodDelegate(
            Type interfaceType,
            MethodInfo sourceMethodInfo,
            MethodContext context,
            ILGenerator generator,
            Type closureFinalType,
            bool hasParameters,
            bool returnsVoid)
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

        private static void EmitMethodName(MethodInfo sourceMethodInfo, ProxyMethodParameter parameter, ILGenerator generator)
        {
            if (parameter.ParameterInfo.ParameterType != typeof(string))
            {
                throw new Exception($"Proxy method parameter \"{parameter.ParameterInfo.Name}\" is attributed with TypeName but it is not a string.");
            }

            generator.Emit(OpCodes.Ldstr, sourceMethodInfo.Name);
        }

        private static void EmitParametersClosure(ProxyMethodParameter parameter, Type closureFinalType, ILGenerator generator, ParameterInfo[] parameters)
        {
            if (parameters.Length == 0)
            {
                throw new Exception(
                    $"Proxy method parameter \"{parameter.ParameterInfo.Name}\" is specified with a closure parameter, even though the method does not have any parameters.");
            }

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

        private static void EmitTypeName(Type interfaceType, ProxyMethodParameter parameter, ILGenerator generator)
        {
            if (parameter.ParameterInfo.ParameterType != typeof(string))
            {
                throw new Exception($"Proxy method parameter \"{parameter.ParameterInfo.Name}\" is attributed with TypeName but it is not a string.");
            }

            generator.Emit(OpCodes.Ldstr, interfaceType.FullName);
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
            bool isAsyncMethod,
            Type interfaceType)
        {
            IEnumerable<MethodInfo> proxyMethods = TypeMethodInfoLookup.GetOrAdd(
                parentType,
                pt => pt.GetInstanceMethods().Where(im => im.GetCustomAttributes(typeof(ProxyMethodAttribute), true).Any()).ToArray());

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
            }
            else
            {
                proxyMethods = proxyMethods.Where(method => method.ReturnType == typeof(void) || method.ReturnType == typeof(Task));
            }

            // get Task<> return type if needed
            proxyMethods = proxyMethods.Where(method => method.ReturnType.Is<Task>() == isAsyncMethod);

            var minimumParameterCount = 1 + (hasParameters ? 1 : 0);

            proxyMethods = proxyMethods.Select(
                    m =>
                        {
                            var parameters = m.GetParameters();

                            return new
                                       {
                                           Method = m,
                                           IsAsync = isAsyncMethod,
                                           HasParameters = hasParameters,
                                           HasReturnValue = hasReturnValue,
                                           ReturnType = returnType,
                                           InterfaceType = interfaceType,
                                           Parameters = parameters.Select(
                                                   p => new
                                                            {
                                                                Parameter = p,
                                                                ProxyMethodParameterType =
                                                                    p.GetCustomAttribute<ProxyMethodParameterTypeAttribute>()?.ParameterType ?? ProxyMethodParameterType.Unknown
                                                            })
                                               .ToArray()
                                       };
                        })
                .Where(method => method.Parameters.Length >= minimumParameterCount)
                .Where(
                    method =>
                        {
                            // Ensure the delegate is compatible
                            var parameter = method.Parameters.FirstOrDefault(p => p.ProxyMethodParameterType == ProxyMethodParameterType.MethodDelegate);

                            if (parameter == null)
                            {
                                return false;
                            }

                            var parameterType = parameter.Parameter.ParameterType;
                            var genericArgumentTypes = parameterType.GetGenericArguments();

                            Type delegateInterfaceType = null;

                            if (method.HasReturnValue)
                            {
                                var returnValueArgument = genericArgumentTypes.Last();

                                if (method.IsAsync)
                                {
                                    if (returnValueArgument.IsGenericType == false || returnValueArgument.GetGenericTypeDefinition() != typeof(Task<>))
                                    {
                                        return false;
                                    }
                                }
                                else if (returnValueArgument != method.ReturnType && returnValueArgument.IsGenericParameter == false)
                                {
                                    return false;
                                }

                                delegateInterfaceType = genericArgumentTypes[genericArgumentTypes.Length - 2];
                            }
                            else
                            {
                                if (method.IsAsync)
                                {
                                    if (method.ReturnType != typeof(Task))
                                    {
                                        return false;
                                    }

                                    delegateInterfaceType = genericArgumentTypes[genericArgumentTypes.Length - 2];
                                }
                                else
                                {
                                    if (method.ReturnType != typeof(void))
                                    {
                                        return false;
                                    }

                                    delegateInterfaceType = genericArgumentTypes[genericArgumentTypes.Length - 1];
                                }
                            }

                            if (delegateInterfaceType != method.InterfaceType)
                            {
                                return false;
                            }

                            return true;
                        })
                .Where(
                    method =>
                        {
                            if (method.HasParameters)
                            {
                                return method.Parameters.Count(p => p.ProxyMethodParameterType == ProxyMethodParameterType.ParametersClosure) == 1;
                            }

                            return method.Parameters.All(p => p.ProxyMethodParameterType != ProxyMethodParameterType.ParametersClosure);
                        })
                .OrderByDescending(method => method.Parameters.Length)
                .Select(method => method.Method);

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

        private struct GetProxyMethodInfoParameter
        {
            public ParameterInfo Parameter { get; set; }

            public ProxyMethodParameterTypeAttribute ProxyMethodParameterTypeAttribute { get; set; }
        }

        public class MethodContext : BaseMethodContext
        {
            public Type ClosureFinalType { get; set; }

            public MethodInfo FinalDelegateMethodInfo { get; set; }
        }

        public class TypeContext : BaseTypeContext<TypeContext, MethodContext>
        {
        }

        private class GetProxyMethodInfo
        {
            public bool HasParameters { get; set; }

            public bool IsAsync { get; set; }

            public MethodInfo MethodInfo { get; set; }

            public GetProxyMethodInfoParameter[] Parameters { get; set; }
        }
    }
}