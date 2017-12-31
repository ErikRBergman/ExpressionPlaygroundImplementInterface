namespace ExpressionPlayground
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    public class ProxyGenerator<TInterfaceToImplement> : ProxyGenerator
    {
        public ProxyGenerator(string assemblyName)
            : base(typeof(TInterfaceToImplement), assemblyName)
        {
        }
    }

    public class ProxyGenerator
    {
        private readonly AssemblyBuilder assemblyBuilder;

        private readonly AssemblyName assemblyName;

        private readonly Type baseClass;

        private readonly Type interfaceToImplementType;

        private readonly ModuleBuilder moduleBuilder;

        public ProxyGenerator(Type interfaceToImplementType, string assemblyName = null)
        {
            this.interfaceToImplementType = interfaceToImplementType;
            var domain = AppDomain.CurrentDomain;
            this.assemblyName = new AssemblyName(assemblyName ?? Guid.NewGuid().ToString("N"));

            this.assemblyBuilder = domain.DefineDynamicAssembly(this.assemblyName, AssemblyBuilderAccess.RunAndSave);
            this.moduleBuilder = this.assemblyBuilder.DefineDynamicModule(this.assemblyName.Name, this.assemblyName.Name + ".dll");

            this.baseClass = typeof(ProxyBase<>).MakeGenericType(this.interfaceToImplementType);
        }

        public Type GenerateProxy()
        {
            var typeName = "ExpressionPlayground." + this.interfaceToImplementType + "+serviceInterfaceProxy";
            var typeBuilder = this.moduleBuilder.DefineType(typeName, TypeAttributes.Public);
            typeBuilder.SetParent(this.baseClass);
            typeBuilder.AddInterfaceImplementation(this.interfaceToImplementType);

            // TODO: Add the outer class parameter to the constructor and add code to store it
            this.CreateSuperClassConstructorCalls(typeBuilder, this.baseClass).ToArray();

            var interfaces = this.GetInterfaces(this.interfaceToImplementType);

            var result = this.ImplementInterfaceProxy(interfaces, typeBuilder);

            var newType = typeBuilder.CreateType();

            this.assemblyBuilder.Save(this.assemblyName.Name + ".dll");

            return newType;
        }

        private static Type CreateClosureType(string closureTypeName, ModuleBuilder moduleBuilder, MethodInfo sourceMethodInfo)
        {
            // var closureTypeBuilder = typeBuilder.DefineNestedType(methodInfo.Name + "_" + Guid.NewGuid().ToString("N"), TypeAttributes.NestedPrivate | TypeAttributes.Sealed);
            var parameters = sourceMethodInfo.GetParameters();

            var closureTypeBuilder = moduleBuilder.DefineType(closureTypeName, TypeAttributes.Public | TypeAttributes.Sealed);

            var genericArgumentArray = sourceMethodInfo.GetGenericArguments();
            var closureGenericArguments = genericArgumentArray.Length == 0
                                              ? Array.Empty<GenericTypeParameterBuilder>()
                                              : closureTypeBuilder.DefineGenericParameters(genericArgumentArray.Select(ga => ga.Name).ToArray());

            foreach (var parameter in parameters)
            {
                var parameterType = parameter.ParameterType;

                var index = Array.IndexOf(genericArgumentArray, parameterType);
                if (index != -1)
                {
                    parameterType = closureGenericArguments[index];
                }

                closureTypeBuilder.DefineField(parameter.Name, parameterType, FieldAttributes.Public);
            }

            closureTypeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            var closureType = closureTypeBuilder.CreateType();

            return closureType;
        }

        private static MethodBuilder CreateDelegateMethod(
            string delegateMethodName,
            TypeBuilder typeBuilder,
            MethodInfo methodInfo,
            Type closureType,
            Type interfaceToImplementType)
        {
            var closureFinalType = closureType;

            var genericArguments = methodInfo.GetGenericArguments();

            if (genericArguments.Length > 0)
            {
                closureFinalType = closureType.MakeGenericType(genericArguments);
            }

            // Create delegate method - to be able to pass parameters 
            var delegateMethodBuilder = typeBuilder.DefineMethod(
                delegateMethodName,
                MethodAttributes.Private,
                methodInfo.ReturnType,
                new[] { closureFinalType, interfaceToImplementType });

            if (genericArguments.Length > 0)
            {
                delegateMethodBuilder.DefineGenericParameters(genericArguments.Select(ga => ga.Name).ToArray());
            }

            // Get arguments from the closure type
            var closureFields = closureFinalType.GetFields();

            var parameters = methodInfo.GetParameters();

            var generator = delegateMethodBuilder.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_2); // get the service parameter

            for (var i = 0; i < parameters.Length; i++)
            {
                generator.Emit(OpCodes.Ldarg_1); // the closure type parameter
                generator.Emit(OpCodes.Ldfld, closureFields[i]); // closure field i
            }

            generator.EmitCall(OpCodes.Callvirt, methodInfo, null); // call the inner method
            generator.Emit(OpCodes.Ret);

            return delegateMethodBuilder;
        }

        private IEnumerable<ConstructorBuilder> CreateSuperClassConstructorCalls(TypeBuilder typeBuilder, Type baseClass)
        {
            foreach (var baseConstructor in baseClass.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var parameters = baseConstructor.GetParameters();
                if (parameters.Length > 0 && parameters.Last().IsDefined(typeof(ParamArrayAttribute), false))
                {
                    throw new InvalidOperationException("Variadic constructors are not supported"); // LOCSTR
                }

                var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();

                var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, baseConstructor.CallingConvention, parameterTypes);
                for (var i = 0; i < parameters.Length; ++i)
                {
                    var parameter = parameters[i];
                    var pb = ctor.DefineParameter(i + 1, parameter.Attributes, parameter.Name);
                    if (((int)parameter.Attributes & (int)ParameterAttributes.HasDefault) != 0)
                    {
                        pb.SetConstant(parameter.RawDefaultValue);
                    }
                }

                var getIL = ctor.GetILGenerator();
                getIL.Emit(OpCodes.Nop);

                getIL.Emit(OpCodes.Ldarg_0);
                for (var i = 1; i <= parameters.Length; ++i)
                {
                    getIL.Emit(OpCodes.Ldarg, i);
                }

                getIL.Emit(OpCodes.Call, baseConstructor);

                getIL.Emit(OpCodes.Ret);

                yield return ctor;
            }
        }

        private void EmitMethodImplementationWithoutParameters(MethodInfo mi, MethodBuilder mb)
        {
            throw new NotImplementedException();
        }

        private void EmitMethodImplementationWithParameters(
            MethodInfo mi,
            MethodBuilder mb,
            Type closureType,
            MethodBuilder delegateMethodBuilder,
            Type interfaceType,
            Type baseClass)
        {
            var generator = mb.GetILGenerator();

            // Create the closure type and instantiate it
            var finalClosureType = closureType;

            var genericArguments = mi.GetGenericArguments();
            var parameters = mi.GetParameters();

            MethodInfo finalDelegateMethod = delegateMethodBuilder;

            if (genericArguments.Length > 0)
            {
                finalClosureType = closureType.MakeGenericType(genericArguments);
                finalDelegateMethod = finalDelegateMethod.MakeGenericMethod(genericArguments);
            }

            var closureVariable = generator.DeclareLocal(finalClosureType);
            var closureConstructor = finalClosureType.GetConstructors().Single();

            generator.Emit(OpCodes.Newobj, closureConstructor);

            // Populate the closure
            var closureFields = finalClosureType.GetFields();

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
            var delegateType = typeof(Func<,,>).MakeGenericType(finalClosureType, interfaceType, mi.ReturnType);
            var delegateConstructor = delegateType.GetConstructors().Single(p => p.GetParameters().Length == 2);
            generator.Emit(OpCodes.Newobj, delegateConstructor);

            var executeAsyncMethods = baseClass.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(method => method.Name == "ExecuteAsync");

            MethodInfo executeAsync = null;

            // If it has a return value
            if (mi.ReturnType.GenericTypeArguments.Length == 1)
            {
                executeAsync = executeAsyncMethods.Single(method => method.ReturnType.ContainsGenericParameters && method.GetParameters().Length == 2);
                executeAsync = executeAsync.MakeGenericMethod(finalClosureType, mi.ReturnType.GenericTypeArguments[0]);
            }
            else
            {
                executeAsync = executeAsyncMethods.Single(method => method.ReturnType.ContainsGenericParameters == false && method.GetParameters().Length == 1);
                executeAsync = executeAsync.MakeGenericMethod(finalClosureType);
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

        private ImmutableList<string> GenerateMethods(TypeBuilder typeBuilder, Type @interface, ImmutableList<string> usedNames)
        {
            foreach (var methodToImplementMethodInfo in @interface.GetMethods())
            {
                var methodParameters = methodToImplementMethodInfo.GetParameters();
                var genericArguments = methodToImplementMethodInfo.GetGenericArguments();

                var paramNames = string.Join(", ", methodParameters.Select(pi => pi.ParameterType));
                var nameWithParams = string.Concat(methodToImplementMethodInfo.Name, "(", paramNames, ")");
                if (usedNames.Contains(nameWithParams))
                {
                    throw new NotSupportedException(string.Format("Error in interface {1}! Method '{0}' already used in other child interface!", nameWithParams, @interface.Name));
                }

                usedNames = usedNames.Add(nameWithParams);

                var genericArgumentNames = genericArguments.Select(pi => pi.Name).ToArray();

                var interfaceImplementationMethodBuilder = typeBuilder.DefineMethod(
                    methodToImplementMethodInfo.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    methodToImplementMethodInfo.ReturnType,
                    methodParameters.Select(pi => pi.ParameterType).ToArray());

                if (genericArguments.Any())
                {
                    interfaceImplementationMethodBuilder.DefineGenericParameters(genericArgumentNames);
                }

                if (methodParameters.Length > 0)
                {
                    // var closureTypeName = "ExpressionPlayground." + methodToImplementMethodInfo.Name + "_" + Guid.NewGuid().ToString("N");
                    var closureTypeName = "ExpressionPlayground.ExpressionPlayground." + methodToImplementMethodInfo.Name;

                    var closureType = CreateClosureType(closureTypeName, this.moduleBuilder, methodToImplementMethodInfo);

                    var delegateMethodName = methodToImplementMethodInfo.Name + "_delegate_" + Guid.NewGuid().ToString("N");
                    var delegateMethodBuilder = CreateDelegateMethod(delegateMethodName, typeBuilder, methodToImplementMethodInfo, closureType, @interface);

                    // Create interface implementation
                    this.EmitMethodImplementationWithParameters(
                        methodToImplementMethodInfo,
                        interfaceImplementationMethodBuilder,
                        closureType,
                        delegateMethodBuilder,
                        @interface,
                        this.baseClass);
                }
                else
                {
                    this.EmitMethodImplementationWithoutParameters(methodToImplementMethodInfo, interfaceImplementationMethodBuilder);
                }

                // Since we're implementing an interface
                typeBuilder.DefineMethodOverride(interfaceImplementationMethodBuilder, methodToImplementMethodInfo);
            }

            return usedNames;
        }

        private ImmutableHashSet<Type> GetInterfaces(Type interfaceType, ImmutableHashSet<Type> interfaces = null)
        {
            interfaces = interfaces ?? ImmutableHashSet<Type>.Empty.Add(interfaceType);
            interfaces = interfaceType.GetInterfaces().Aggregate(interfaces, (current, i) => current.Add(i));

            return interfaces;
        }

        private DynamicImplementInterfaceResult ImplementInterfaceProxy(
            IEnumerable<Type> interfaces,
            TypeBuilder typeBuilder,
            DynamicImplementInterfaceResult? previousResult = null)
        {
            var result = previousResult ?? DynamicImplementInterfaceResult.Empty;
            var innerInstanceFieldInfo = this.baseClass.GetField("inner", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var interfaceType in interfaces)
            {
                result = result.AddUsedNames(this.GenerateMethods(typeBuilder, interfaceType, result.NamesUsed));
                result = result.AddImplementedInterface(interfaceType);
            }

            return result;
        }
    }
}