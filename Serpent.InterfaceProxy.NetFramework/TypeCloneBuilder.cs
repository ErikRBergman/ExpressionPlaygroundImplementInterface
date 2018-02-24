namespace Serpent.InterfaceProxy
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Serpent.InterfaceProxy.Extensions;
    using Serpent.InterfaceProxy.ImplementationBuilders;
    using Serpent.InterfaceProxy.Types;
    using Serpent.InterfaceProxy.Validation;

    public class TypeCloneBuilder<TMethodContext>
    {
        private readonly Func<Type, Type> proxyBaseTypeFactoryFunc;

        public TypeCloneBuilder(Func<Type, Type> parentTypeFactoryFunc)
        {
            this.proxyBaseTypeFactoryFunc = parentTypeFactoryFunc ?? throw new ArgumentNullException(nameof(parentTypeFactoryFunc));
        }

        public TypeCloneBuilder(Type parentType)
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

        private TypeCloneBuilder()
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

        public GenerateTypeResult GenerateProxy(Type interfaceToImplement, string typeName = null)
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

            return new GenerateTypeResult(generatedType, result.InterfacesImplemented, factory);
        }

        protected virtual void EmitMethodImplementation(
            Type interfaceType,
            MethodInfo sourceMethodInfo,
            MethodBuilder methodBuilder,
            TMethodContext context,
            Type parentType)
        {
        }

        private static MethodBuilder CreateMethod(
            TypeBuilder typeBuilder,
            MethodInfo sourceMethodInfo,
            ParameterInfo[] parameters,
            Type[] genericArguments,
            string[] genericArgumentNames)
        {
            if (sourceMethodInfo.Name == "GenericsAndVarArgs")
            {
            }

            var interfaceImplementationMethodBuilder = typeBuilder.DefineMethod(
                sourceMethodInfo.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final,
                sourceMethodInfo.ReturnType,
                parameters.Select(pi => pi.ParameterType).ToArray());

            if (genericArguments.Any())
            {
                var genericParameters = interfaceImplementationMethodBuilder.DefineGenericParameters(genericArgumentNames);
                var substituteTypes = genericArguments.ZipToDictionaryMap(genericParameters);

                var returnType = sourceMethodInfo.ReturnType;

                var newReturnType = substituteTypes.GetSubstitute(returnType);

                interfaceImplementationMethodBuilder.SetReturnType(newReturnType);
            }

            UpdateParameterAttributes(interfaceImplementationMethodBuilder, parameters);

            return interfaceImplementationMethodBuilder;
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

        private static void UpdateParameterAttributes(MethodBuilder interfaceImplementationMethodBuilder, ParameterInfo[] parameters)
        {
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
        }

        protected virtual TMethodContext CreateMethodContext(
            Type @interface,
            TypeBuilder typeBuilder,
            MethodInfo sourceMethodInfo,
            ParameterInfo[] parameters,
            Type[] genericArguments)
        {
            return default(TMethodContext);
        }

        private ImmutableList<string> ImplementInterfaceMethod(
            TypeBuilder typeBuilder,
            Type @interface,
            ImmutableList<string> usedNames,
            Type parentType)
        {
            foreach (var sourceMethod in @interface.GetMethods())
            {
                if (sourceMethod.Name == "Result_Parameters")
                {
                }

                var parameters = sourceMethod.GetParameters();
                var genericArguments = sourceMethod.GetGenericArguments();

                var paramNames = string.Join(", ", parameters.Select(pi => pi.ParameterType));
                var nameWithParams = string.Concat(sourceMethod.Name, "(", paramNames, ")");
                if (usedNames.Contains(nameWithParams))
                {
                    throw new NotSupportedException(string.Format("Error in interface {1}! Method '{0}' already used in other child interface!", nameWithParams, @interface.Name));
                }

                usedNames = usedNames.Add(nameWithParams);

                var genericArgumentNames = genericArguments.Select(pi => pi.Name).ToArray();

                if (sourceMethod.Name == "GenericsAndVarArgs")
                {
                }

                var methodContext = this.CreateMethodContext(@interface, typeBuilder, sourceMethod, parameters, genericArguments);

                // Methods.MethodBuilder.EmitMethodImplementation(@interface, method, typeBuilder, genericArguments, finalClosureType, finalDelegateMethod, parentType);
                var interfaceImplementationMethodBuilder = CreateMethod(
                    typeBuilder,
                    sourceMethod,
                    parameters,
                    genericArguments,
                    genericArgumentNames);

                //this.EmitMethodImplementation(@interface, method, interfaceImplementationMethodBuilder, finalClosureType, finalDelegateMethod, parentType);
                
                this.EmitMethodImplementation(@interface, sourceMethod, interfaceImplementationMethodBuilder, methodContext, parentType);
            }

            return usedNames;
        }

        private ImplementInterfaceMethodResult ImplementInterfaceMethods(TypeBuilder typeBuilder, IEnumerable<Type> interfaces, Type parentType)
        {
            var result = ImplementInterfaceMethodResult.Empty;

            foreach (var interfaceType in interfaces)
            {
                result = result.AddUsedNames(
                    this.ImplementInterfaceMethod(
                        typeBuilder,
                        interfaceType,
                        result.NamesUsed,
                        parentType));
                result = result.AddImplementedInterface(interfaceType);
            }

            return result;
        }
    }
}