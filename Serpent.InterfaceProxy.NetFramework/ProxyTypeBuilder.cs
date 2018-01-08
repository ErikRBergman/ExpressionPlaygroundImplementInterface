// ReSharper disable StyleCop.SA1402
// ReSharper disable ParameterHidesMember

namespace Serpent.InterfaceProxy
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading.Tasks;

    using Serpent.InterfaceProxy.Extensions;
    using Serpent.InterfaceProxy.ImplementationBuilders;
    using Serpent.InterfaceProxy.Types;
    using Serpent.InterfaceProxy.Validation;

    public class ProxyTypeBuilder
    {
        private readonly Func<Type, Type> proxyBaseTypeFactoryFunc;

        public ProxyTypeBuilder(Func<Type, Type> parentTypeFactoryFunc)
        {
            this.proxyBaseTypeFactoryFunc = parentTypeFactoryFunc ?? throw new ArgumentNullException(nameof(parentTypeFactoryFunc));
        }

        public ProxyTypeBuilder(Type parentType)
            : this()
        {
            Validator.Default.IsNotNull(parentType, nameof(parentType));

            if (parentType.ContainsGenericParameters)
            {
                var parentGenericArguments = parentType.GetGenericArguments();

                if (parentGenericArguments.Length != 1)
                {
                    throw new Exception("Parent type has more than one generic argument. Use the constructor with a factory function instead.");
                }

                this.proxyBaseTypeFactoryFunc = t => parentType.MakeGenericType(t);
            }
            else
            {
                this.proxyBaseTypeFactoryFunc = t => parentType;
            }
        }

        private ProxyTypeBuilder()
        {
            this.Namespace = this.Namespace ?? DefaultValues.DefaultTypeNamespace;

            this.ModuleBuilder = DefaultValues.DefaultModuleBuilder;

            // Default delegates for names
            this.ClosureTypeNameSelector = (@interface, methodInfo, @namespace) =>
                @namespace + "." + @interface.Name + "." + methodInfo.Name + "_Closure_" + Guid.NewGuid().ToString("N");

            this.ProxyTypeNameSelectorFunc = (@interface, @namespace) => @namespace + "." + @interface.Name + "+proxy";
        }

        public Func<Type, MethodInfo, string, string> ClosureTypeNameSelector { get; set; }

        public ModuleBuilder ModuleBuilder { get; set; }

        public string Namespace { get; set; }

        public Func<Type, string, string> ProxyTypeNameSelectorFunc { get; set; }

        public static IEnumerable<MethodInfo> GetExecuteAsyncMethods(Type parentType)
        {
            return parentType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(method => method.Name == "ExecuteAsync" || method.Name == "Execute");
        }

        public GenerateProxyResult GenerateProxy(Type interfaceToImplement, string typeName = null)
        {
            var validator = Validator.Default;

            validator.IsNotNull(interfaceToImplement, nameof(interfaceToImplement)).IsInterface(interfaceToImplement, nameof(interfaceToImplement));

            var parentType = this.proxyBaseTypeFactoryFunc(interfaceToImplement);

            typeName = typeName ?? this.ProxyTypeNameSelectorFunc(interfaceToImplement, this.Namespace);

            var typeBuilder = this.ModuleBuilder.DefineType(typeName, TypeAttributes.Public, parentType);
            typeBuilder.AddInterfaceImplementation(interfaceToImplement);

            DefaultConstructorGenerator.CreateDefaultConstructors(typeBuilder, parentType);

            var result = this.ImplementInterfaceMethods(typeBuilder, interfaceToImplement.GetAllInterfaces(), parentType);

            var generatedType = typeBuilder.CreateTypeInfo();

            var factory = GenerateFactoryDelegate(interfaceToImplement, generatedType);

            return new GenerateProxyResult(generatedType, result.InterfacesImplemented, factory);
        }

        private static MethodBuilder CreateMethod(
            TypeBuilder typeBuilder,
            Type @interface,
            Type parentType,
            MethodInfo method,
            ParameterInfo[] parameters,
            Type[] genericArguments,
            string[] genericArgumentNames,
            Type finalClosureType,
            MethodInfo finalDelegateMethod)
        {
            if (method.Name == "GenericsAndVarArgs")
            {
            }

            var interfaceImplementationMethodBuilder = typeBuilder.DefineMethod(
                method.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final,
                method.ReturnType,
                parameters.Select(pi => pi.ParameterType).ToArray());

            if (genericArguments.Any())
            {
                var genericParameters = interfaceImplementationMethodBuilder.DefineGenericParameters(genericArgumentNames);
                var substituteTypes = genericArguments.ZipToDictionaryMap(genericParameters);

                var returnType = method.ReturnType;

                var newReturnType = TypeSubstitutor.GetSubstitutedType(returnType, substituteTypes);

                interfaceImplementationMethodBuilder.SetReturnType(newReturnType);
            }

            for (var i = 0; i < parameters.Length; i++)
            {
                var builder = interfaceImplementationMethodBuilder.DefineParameter(i + 1, parameters[i].Attributes, parameters[i].Name);

                var paramArrayAttribute = parameters[i].GetCustomAttribute<ParamArrayAttribute>();

                if (paramArrayAttribute != null)
                {
                    var attributeBuilder = new CustomAttributeBuilder(
                        typeof(ParamArrayAttribute).GetConstructors().First(c => c.IsPublic && c.GetParameters().Length == 0),
                        new object[0]);
                    builder.SetCustomAttribute(attributeBuilder);
                }

                if (parameters[i].Attributes.HasFlag(ParameterAttributes.HasDefault))
                {
                    builder.SetConstant(parameters[i].DefaultValue);
                }
            }

            EmitMethodImplementation(@interface, method, interfaceImplementationMethodBuilder, finalClosureType, finalDelegateMethod, parentType);
            return interfaceImplementationMethodBuilder;
        }

        private static void EmitMethodImplementation(
            Type interfaceType,
            MethodInfo sourceMethodInfo,
            MethodBuilder methodBuilder,
            Type closureFinalType,
            MethodInfo finalDelegateMethod,
            Type parentType)
        {
            var generator = methodBuilder.GetILGenerator();
            var parameters = sourceMethodInfo.GetParameters();

            var hasParameters = parameters.Length != 0;

            var isAsyncMethod = typeof(Task).IsAssignableFrom(sourceMethodInfo.ReturnType);

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

            // Start of call to the proxy method (being called in the end of this method). Moved ldarg_0 here to optimize
            generator.Emit(OpCodes.Ldarg_0); // this (from the current method)

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

            // Second parameter to the Func<*> constructor
            // get the address of this.DelegateMethodAsync
            generator.Emit(OpCodes.Ldarg_0); // this.
            generator.Emit(OpCodes.Ldftn, finalDelegateMethod); // the delegate method

            // Instantiate the Func<*>
            var delegateType = GetDelegateType(interfaceType, sourceMethodInfo, closureFinalType, hasParameters, returnsVoid);

            var delegateConstructor = delegateType.GetConstructors().Single(p => p.GetParameters().Length == 2);
            generator.Emit(OpCodes.Newobj, delegateConstructor);

            var proxyMethod = GetProxyMethod(closureFinalType, parentType, hasParameters, hasReturnValue, returnType, hasGenericReturnValueArguments, isAsyncMethod);

            generator.EmitCall(OpCodes.Call, proxyMethod, null); // call the proxy method
            generator.Emit(OpCodes.Ret);
        }

        private static Delegate GenerateFactoryDelegate(Type interfaceToImplement, Type generatedType)
        {
            var parameterTypes = new[] { interfaceToImplement };

            var builder = new DynamicMethod("DefaultTypeFactory", interfaceToImplement, parameterTypes);
            var generator = builder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);

            var constructor = generatedType.GetConstructor(parameterTypes);

            if (constructor == null)
            {
                return null;
            }

            generator.Emit(OpCodes.Newobj, constructor);
            generator.Emit(OpCodes.Ret);

            return builder.CreateDelegate(typeof(Func<,>).MakeGenericType(interfaceToImplement, interfaceToImplement));
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
            Func<Type, MethodInfo, TypeBuilder> createClosureTypeFunc,
            ParameterInfo[] parameters,
            MethodInfo method,
            Type[] genericArguments)
        {
            Type closureFinalType = null;

            if (parameters.Length > 0)
            {
                var closureTypeBuilder = createClosureTypeFunc(@interface, method);
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
            Type closureFinalType,
            Type parentType,
            bool hasParameters,
            bool hasReturnValue,
            Type returnType,
            bool hasGenericReturnValueArguments,
            bool isAsyncMethod)
        {
            var proxyMethods = GetExecuteAsyncMethods(parentType);

            var proxyMethodGenericParameters = ImmutableArray<Type>.Empty;

            if (hasParameters)
            {
                proxyMethodGenericParameters = proxyMethodGenericParameters.Add(closureFinalType);
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
            proxyMethods = proxyMethods.Where(method => typeof(Task).IsAssignableFrom(method.ReturnType) == isAsyncMethod);

            var executeAsyncParameterCount = 1 + (hasParameters ? 1 : 0);

            proxyMethods = proxyMethods.Where(method => method.GetParameters().Length == executeAsyncParameterCount);

            var executeAsync = proxyMethods.Single();

            if (proxyMethodGenericParameters.Length > 0)
            {
                executeAsync = executeAsync.MakeGenericMethod(proxyMethodGenericParameters.ToArray());
            }

            return executeAsync;
        }

        private static ImmutableList<string> ImplementInterfaceMethod(
            TypeBuilder typeBuilder,
            Type @interface,
            ImmutableList<string> usedNames,
            Type parentType,
            Func<Type, MethodInfo, TypeBuilder> createClosureTypeFunc)
        {
            foreach (var method in @interface.GetMethods())
            {
                if (method.Name == "Result_Parameters")
                {
                }

                var parameters = method.GetParameters();
                var genericArguments = method.GetGenericArguments();

                var paramNames = string.Join(", ", parameters.Select(pi => pi.ParameterType));
                var nameWithParams = string.Concat(method.Name, "(", paramNames, ")");
                if (usedNames.Contains(nameWithParams))
                {
                    throw new NotSupportedException(string.Format("Error in interface {1}! Method '{0}' already used in other child interface!", nameWithParams, @interface.Name));
                }

                usedNames = usedNames.Add(nameWithParams);

                var genericArgumentNames = genericArguments.Select(pi => pi.Name).ToArray();

                if (method.Name == "GenericsAndVarArgs")
                {
                }

                var finalClosureType = GetFinalClosureType(@interface, createClosureTypeFunc, parameters, method, genericArguments);
                var finalDelegateMethod = GetFinalDelegateMethod(typeBuilder, @interface, method, finalClosureType, genericArguments);

                // Methods.MethodBuilder.EmitMethodImplementation(@interface, method, typeBuilder, genericArguments, finalClosureType, finalDelegateMethod, parentType);
                CreateMethod(
                    typeBuilder,
                    @interface,
                    parentType,
                    method,
                    parameters,
                    genericArguments,
                    genericArgumentNames,
                    finalClosureType,
                    finalDelegateMethod);
            }

            return usedNames;
        }

        private ImplementInterfaceMethodResult ImplementInterfaceMethods(TypeBuilder typeBuilder, IEnumerable<Type> interfaces, Type parentType)
        {
            var result = ImplementInterfaceMethodResult.Empty;

            foreach (var interfaceType in interfaces)
            {
                result = result.AddUsedNames(
                    ImplementInterfaceMethod(
                        typeBuilder,
                        interfaceType,
                        result.NamesUsed,
                        parentType,
                        (@interface, method) =>
                            {
                                var closureTypeName = this.ClosureTypeNameSelector(@interface, method, this.Namespace);
                                return ClosureBuilder.CreateClosureTypeBuilder(this.ModuleBuilder, closureTypeName);
                            }));
                result = result.AddImplementedInterface(interfaceType);
            }

            return result;
        }
    }
}