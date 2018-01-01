﻿// ReSharper disable StyleCop.SA1402

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
        private Func<Type, MethodInfo, string, string> closureTypeNameSelector;

        private ModuleBuilder moduleBuilder;

        private string namespaceName;

        private Type parentType;

        private Func<Type, string, string> proxyTypeNameSelectorFunc;

        public ProxyTypeBuilder()
        {
            this.namespaceName = this.namespaceName ?? typeof(ProxyTypeBuilder).Namespace + ".GeneratedTypes";

            this.moduleBuilder = DefaultValues.DefaultModuleBuilder;

            // Default delegates for names
            this.closureTypeNameSelector = (@interface, methodInfo, @namespace) => @namespace + "." + @interface.Name + "." + methodInfo.Name + "_" + "_" + Guid.NewGuid().ToString("N");
            this.proxyTypeNameSelectorFunc = (@interface, @namespace) => @namespace + "." + @interface.Name + "+proxy";
        }

        public ProxyTypeBuilder ClosureTypeNameSelector(Func<Type, MethodInfo, string, string> closureTypeNameSelector)
        {
            this.closureTypeNameSelector = closureTypeNameSelector ?? throw new ArgumentNullException(nameof(closureTypeNameSelector));
            return this;
        }

        public GeneratedProxy GenerateProxy(Type interfaceToImplement)
        {
            Validator.Default.IsNotNull(interfaceToImplement, nameof(interfaceToImplement)).IsInterface(interfaceToImplement, nameof(interfaceToImplement));

            this.parentType = typeof(ProxyBase<>).MakeGenericType(interfaceToImplement);

            var typeName = this.proxyTypeNameSelectorFunc(interfaceToImplement, this.namespaceName);

            var typeBuilder = this.moduleBuilder.DefineType(typeName, TypeAttributes.Public, this.parentType);
            typeBuilder.AddInterfaceImplementation(interfaceToImplement);

            DefaultConstructorGenerator.CreateSuperClassConstructorCalls(typeBuilder, this.parentType);

            var result = this.ImplementInterfaceMethods(typeBuilder, interfaceToImplement.GetAllInterfaces());

            var generatedType = typeBuilder.CreateType();
            return new GeneratedProxy(generatedType, result.InterfacesImplemented);
        }

        public ProxyTypeBuilder ModuleBuilder(ModuleBuilder moduleBuilder)
        {
            this.moduleBuilder = moduleBuilder;
            return this;
        }

        public ProxyTypeBuilder Namespace(string @namespace)
        {
            this.namespaceName = @namespace;
            return this;
        }

        public ProxyTypeBuilder ProxyTypeNameSelector(Func<Type, string, string> proxyTypeNameSelectorFunc)
        {
            this.proxyTypeNameSelectorFunc = proxyTypeNameSelectorFunc;
            return this;
        }

        private static void EmitMethodImplementationWithoutParameters(MethodInfo mi, MethodBuilder mb)
        {
            throw new NotImplementedException();
        }

        private static void EmitMethodImplementationWithParameters(
            Type interfaceType,
            MethodInfo sourceMethodInfo,
            MethodBuilder methodBuilder,
            Type closureFinalType,
            MethodBuilder delegateMethodBuilder,
            Type parentType)
        {
            var genericArguments = sourceMethodInfo.GetGenericArguments();

            var finalDelegateMethod = delegateMethodBuilder.MakeGenericMethodIfNecessary(genericArguments);

            var generator = methodBuilder.GetILGenerator();

            var closureVariable = generator.DeclareLocal(closureFinalType);
            var closureConstructor = closureFinalType.GetConstructors().Single();

            generator.Emit(OpCodes.Newobj, closureConstructor);

            // Populate the closure
            var parameters = sourceMethodInfo.GetParameters();
            var closureFields = closureFinalType.GetFields();

            for (var i = 0; i < parameters.Length; i++)
            {
                generator.Emit(OpCodes.Dup);
                generator.Emit(OpCodes.Ldarg, i + 1); // argument i+1 (0 is this.)
                generator.Emit(OpCodes.Stfld, closureFields[i]);
            }

            // Store the closure in our local variable
            generator.Emit(OpCodes.Stloc, closureVariable);

            // Call this.ExecuteAsync
            generator.Emit(OpCodes.Ldarg_0); // this (from the current method)

            // First parameter to the Func<,,> constructor
            generator.Emit(OpCodes.Ldloc_0);

            // Second parameter to the Func<,,> constructor
            // get the address of this.DelegateMethodAsync
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldftn, finalDelegateMethod);

            // Instantiate the Func<,,>
            var delegateType = typeof(Func<,,>).MakeGenericType(closureFinalType, interfaceType, sourceMethodInfo.ReturnType);
            var delegateConstructor = delegateType.GetConstructors().Single(p => p.GetParameters().Length == 2);
            generator.Emit(OpCodes.Newobj, delegateConstructor);

            var executeAsyncMethods = GetExecuteAsyncMethods(parentType);

            MethodInfo executeAsync = null;

            // If it has a return value
            if (sourceMethodInfo.ReturnType.GenericTypeArguments.Length == 1)
            {
                executeAsync = executeAsyncMethods.Single(method => method.ReturnType.ContainsGenericParameters && method.GetParameters().Length == 2);
                executeAsync = executeAsync.MakeGenericMethod(closureFinalType, sourceMethodInfo.ReturnType.GenericTypeArguments[0]);
            }
            else
            {
                executeAsync = executeAsyncMethods.Single(method => method.ReturnType.ContainsGenericParameters == false && method.GetParameters().Length == 1);
                executeAsync = executeAsync.MakeGenericMethod(closureFinalType);
            }

            generator.EmitCall(OpCodes.Call, executeAsync, null); // call the inner method
            generator.Emit(OpCodes.Ret);

            ////This code calls the inner method instead
            ////generator.Emit(OpCodes.Ldarg_0); // this (from the current method)
            ////generator.Emit(OpCodes.Ldfld, innerInstanceFieldInfo); // .inner

            ////for (int i = 0; i < parameters.Length; i++)
            ////{
            ////    generator.Emit(OpCodes.Ldarg, i + 1); // parameter x
            ////}

            ////generator.EmitCall(OpCodes.Callvirt, mi, null); // call the inner method
            ////generator.Emit(OpCodes.Ret);
        }

        private static IEnumerable<MethodInfo> GetExecuteAsyncMethods(Type parentType)
        {
            return parentType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(method => method.Name == "ExecuteAsync");
        }

        private static ImmutableList<string> ImplementInterfaceMethod(TypeBuilder typeBuilder, Type @interface, ImmutableList<string> usedNames, Type parentType, Func<Type, MethodInfo, TypeBuilder> createClosureTypeFunc)
        {
            foreach (var method in @interface.GetMethods())
            {
                var methodParameters = method.GetParameters();
                var genericArguments = method.GetGenericArguments();

                var paramNames = string.Join(", ", methodParameters.Select(pi => pi.ParameterType));
                var nameWithParams = string.Concat(method.Name, "(", paramNames, ")");
                if (usedNames.Contains(nameWithParams))
                {
                    throw new NotSupportedException(string.Format("Error in interface {1}! Method '{0}' already used in other child interface!", nameWithParams, @interface.Name));
                }

                usedNames = usedNames.Add(nameWithParams);

                var genericArgumentNames = genericArguments.Select(pi => pi.Name).ToArray();

                var interfaceImplementationMethodBuilder = typeBuilder.DefineMethod(
                    method.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    method.ReturnType,
                    methodParameters.Select(pi => pi.ParameterType).ToArray());

                if (genericArguments.Any())
                {
                    interfaceImplementationMethodBuilder.DefineGenericParameters(genericArgumentNames);
                }

                if (methodParameters.Length > 0)
                {
                    var closureTypeBuilder = createClosureTypeFunc(@interface, method);
                    var closureFinalType = ClosureBuilder.CreateClosureType(closureTypeBuilder, method).MakeGenericTypeIfNecessary(genericArguments);

                    var delegateMethodName = method.Name + "_delegate_" + Guid.NewGuid().ToString("N");
                    var delegateMethodBuilder = DelegateBuilder.CreateDelegateMethod(delegateMethodName, typeBuilder, method, closureFinalType, @interface);

                    // Create interface implementation
                    EmitMethodImplementationWithParameters(@interface, method, interfaceImplementationMethodBuilder, closureFinalType, delegateMethodBuilder,  parentType);
                }
                else
                {
                    EmitMethodImplementationWithoutParameters(method, interfaceImplementationMethodBuilder);
                }

                // Since we're implementing an interface
                typeBuilder.DefineMethodOverride(interfaceImplementationMethodBuilder, method);
            }

            return usedNames;
        }

        private ImplementInterfaceMethodResult ImplementInterfaceMethods(
            TypeBuilder typeBuilder,
            IEnumerable<Type> interfaces,
            ImplementInterfaceMethodResult? previousResult = null)
        {
            var result = previousResult ?? ImplementInterfaceMethodResult.Empty;

            foreach (var interfaceType in interfaces)
            {
                result = result.AddUsedNames(
                    ImplementInterfaceMethod(
                        typeBuilder, 
                        interfaceType, 
                        result.NamesUsed,
                        this.parentType,
                        (@interface, method) =>
                        {
                            var closureTypeName = this.closureTypeNameSelector(@interface, method, this.namespaceName);
                            return ClosureBuilder.CreateClosureTypeBuilder(this.moduleBuilder, closureTypeName);
                        }));
                result = result.AddImplementedInterface(interfaceType);
            }

            return result;
        }
    }
}