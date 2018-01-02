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
            return new GeneratedProxy(generatedType, result.InterfacesImplemented);
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

            if (closureFinalType != null)
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
                executeAsync = executeAsyncMethods.Single(method => method.ReturnType.ContainsGenericParameters == false && method.GetParameters().Length == 2);
                executeAsync = executeAsync.MakeGenericMethod(closureFinalType);
            }

            generator.EmitCall(OpCodes.Call, executeAsync, null); // call the inner method
            generator.Emit(OpCodes.Ret);
        }

        private static IEnumerable<MethodInfo> GetExecuteAsyncMethods(Type parentType)
        {
            return parentType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(method => method.Name == "ExecuteAsync");
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

                var interfaceImplementationMethodBuilder = typeBuilder.DefineMethod(
                    method.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    method.ReturnType,
                    parameters.Select(pi => pi.ParameterType).ToArray());

                if (genericArguments.Any())
                {
                    interfaceImplementationMethodBuilder.DefineGenericParameters(genericArgumentNames);
                }

                Type closureFinalType = null;

                if (parameters.Length > 0)
                {
                    TypeBuilder closureTypeBuilder = createClosureTypeFunc(@interface, method);
                    closureFinalType = ClosureBuilder.CreateClosureType(closureTypeBuilder, method).MakeGenericTypeIfNecessary(genericArguments);
                }

                var delegateMethodName = method.Name + "_delegate_" + Guid.NewGuid().ToString("N");
                var delegateMethodBuilder = DelegateBuilder.CreateDelegateMethod(delegateMethodName, typeBuilder, method, closureFinalType, @interface);
                var finalDelegateMethod = delegateMethodBuilder.MakeGenericMethodIfNecessary(genericArguments);

                if (parameters.Length > 0)
                {

                    // Create interface implementation
                    EmitMethodImplementation(@interface, method, interfaceImplementationMethodBuilder, closureFinalType, finalDelegateMethod, parentType);
                }
                else
                {
                    EmitMethodImplementation(@interface, method, interfaceImplementationMethodBuilder, null, finalDelegateMethod, parentType);
                }

                // Since we're implementing an interface
                typeBuilder.DefineMethodOverride(interfaceImplementationMethodBuilder, method);
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