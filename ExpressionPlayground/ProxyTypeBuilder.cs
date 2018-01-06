// ReSharper disable StyleCop.SA1402

// ReSharper disable ParameterHidesMember

namespace ExpressionPlayground
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using ExpressionPlayground.Closures;
    using ExpressionPlayground.Constructors;
    using ExpressionPlayground.Delegates;
    using ExpressionPlayground.Extensions;
    using ExpressionPlayground.Validation;

    public class ProxyTypeBuilder
    {
        public ProxyTypeBuilder()
        {
            this.Namespace = this.Namespace ?? DefaultValues.DefaultTypeNamespace;

            this.ModuleBuilder = DefaultValues.DefaultModuleBuilder;

            // Default delegates for names
            this.ClosureTypeNameSelector = (@interface, methodInfo, @namespace) =>
                @namespace + "." + @interface.Name + "." + methodInfo.Name + "_" + "_" + Guid.NewGuid().ToString("N");
            this.ProxyTypeNameSelectorFunc = (@interface, @namespace) => @namespace + "." + @interface.Name + "+proxy";
        }

        public Func<Type, MethodInfo, string, string> ClosureTypeNameSelector { get; set; }

        public ModuleBuilder ModuleBuilder { get; set; }

        public string Namespace { get; set; }

        public Func<Type, string, string> ProxyTypeNameSelectorFunc { get; set; }

        public static IEnumerable<MethodInfo> GetExecuteAsyncMethods(Type parentType)
        {
            return parentType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(method => method.Name == "ExecuteAsync");
        }

        public GeneratedProxy GenerateProxy(Type interfaceToImplement)
        {
            Validator.Default.IsNotNull(interfaceToImplement, nameof(interfaceToImplement)).IsInterface(interfaceToImplement, nameof(interfaceToImplement));

            var parentType = typeof(ProxyBase<>).MakeGenericType(interfaceToImplement);

            var typeName = this.ProxyTypeNameSelectorFunc(interfaceToImplement, this.Namespace);

            var typeBuilder = this.ModuleBuilder.DefineType(typeName, TypeAttributes.Public, parentType);
            typeBuilder.AddInterfaceImplementation(interfaceToImplement);

            DefaultConstructorGenerator.CreateSuperClassConstructorCalls(typeBuilder, parentType);

            var result = this.ImplementInterfaceMethods(typeBuilder, interfaceToImplement.GetAllInterfaces(), parentType);

            var generatedType = typeBuilder.CreateType();

            var factory = GenerateFactoryDelegate(interfaceToImplement, generatedType);

            return new GeneratedProxy(generatedType, result.InterfacesImplemented, factory);
        }

        private static MethodBuilder CreateMethodUsingILGenerator(
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
            if (method.Name == "Result_NoParameters")
            {
            }

            var interfaceImplementationMethodBuilder = typeBuilder.DefineMethod(
                method.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final,
                method.ReturnType,
                parameters.Select(pi => pi.ParameterType).ToArray());

            if (genericArguments.Any())
            {
                interfaceImplementationMethodBuilder.DefineGenericParameters(genericArgumentNames);
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
            var hasReturnValue = sourceMethodInfo.ReturnType.GenericTypeArguments.Length == 1;

            // only emit instantiating the closure if it's needed
            if (hasParameters)
            {
                var closureVariable = generator.DeclareLocal(closureFinalType);
                var closureConstructor = closureFinalType.GetConstructors().Single();

                generator.Emit(OpCodes.Newobj, closureConstructor);

                // Populate the closure
                var closureFields = closureFinalType.GetFields();

                for (var i = 0; i < parameters.Length; i++)
                {
                    generator.Emit(OpCodes.Dup);
                    generator.Emit(OpCodes.Ldarg, i + 1); // argument i+1 (0 is this.)
                    generator.Emit(OpCodes.Stfld, closureFields[i]);
                }

                // Store the closure in our local variable
                generator.Emit(OpCodes.Stloc, closureVariable);
            }

            // Start of call to this.ExecuteAsync
            generator.Emit(OpCodes.Ldarg_0); // this (from the current method)

            // With no parameters, the closure does not need to be pushed either
            if (hasParameters)
            {
                // First parameter to the Func<*> constructor
                generator.Emit(OpCodes.Ldloc_0);
            }

            // Second parameter to the Func<*> constructor
            // get the address of this.DelegateMethodAsync
            generator.Emit(OpCodes.Ldarg_0); // this.
            generator.Emit(OpCodes.Ldftn, finalDelegateMethod); // the delegate method

            // Instantiate the Func<*>
            Type delegateType;

            if (hasParameters)
            {
                delegateType = typeof(Func<,,>).MakeGenericType(closureFinalType, interfaceType, sourceMethodInfo.ReturnType);
            }
            else
            {
                delegateType = typeof(Func<,>).MakeGenericType(interfaceType, sourceMethodInfo.ReturnType);
            }

            var delegateConstructor = delegateType.GetConstructors().Single(p => p.GetParameters().Length == 2);
            generator.Emit(OpCodes.Newobj, delegateConstructor);

            var executeAsyncMethods = GetExecuteAsyncMethods(parentType);

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

            generator.EmitCall(OpCodes.Call, executeAsync, null); // call the inner method
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

        private static ImmutableList<string> ImplementInterfaceMethod(
            TypeBuilder typeBuilder,
            Type @interface,
            ImmutableList<string> usedNames,
            Type parentType,
            Func<Type, MethodInfo, TypeBuilder> createClosureTypeFunc)
        {
            foreach (var method in @interface.GetMethods())
            {
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

                var finalClosureType = GetFinalClosureType(@interface, createClosureTypeFunc, parameters, method, genericArguments);
                var finalDelegateMethod = GetFinalDelegateMethod(typeBuilder, @interface, method, finalClosureType, genericArguments);

                // Methods.MethodBuilder.EmitMethodImplementation(@interface, method, typeBuilder, genericArguments, finalClosureType, finalDelegateMethod, parentType);
                var interfaceImplementationMethodBuilder = CreateMethodUsingILGenerator(
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