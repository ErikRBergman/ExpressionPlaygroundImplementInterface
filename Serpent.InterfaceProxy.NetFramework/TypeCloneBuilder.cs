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

    public class GenerateTypeParameters
    {
        public GenerateTypeParameters(Type sourceType)
        {
            this.SourceType = sourceType;
        }

        public Type ParentType { get; set; }

        public Type SourceType { get; }

        public string CreatedTypeName { get; set; }

        public bool CreateConstructors { get; set; }
    }

    public class TypeCloneBuilder<TTypeContext, TMethodContext>
    where TTypeContext : BaseTypeCloneTypeContext
    {
        public TypeCloneBuilder()
        {
            this.Namespace = this.Namespace ?? DefaultValues.DefaultTypeNamespace;

            this.ModuleBuilder = DefaultValues.DefaultModuleBuilder;
        }

        public ModuleBuilder ModuleBuilder { get; set; }

        public string Namespace { get; set; }

        protected virtual TTypeContext CreateTypeContext(GenerateTypeParameters parameters, TypeBuilder typeBuilder)
        {
            return default(TTypeContext);
        }

        public virtual GenerateTypeResult GenerateTypeClone(GenerateTypeParameters parameters)
        {
            var validator = Validator.Default;

            validator.IsNotNull(parameters, nameof(parameters));
            validator.IsNotNull(parameters.SourceType, nameof(parameters) + "." + nameof(parameters.SourceType));

            var sourceType = parameters.SourceType;

            validator.IsNotNull(sourceType, nameof(sourceType)).IsInterface(sourceType, nameof(sourceType));

            var typeName = parameters.CreatedTypeName;
            var parentType = parameters.ParentType;

            TypeBuilder typeBuilder;
            if (parentType != null)
            {
                typeBuilder = this.ModuleBuilder.DefineType(typeName, TypeAttributes.Public, parentType);
            }
            else
            {
                typeBuilder = this.ModuleBuilder.DefineType(typeName, TypeAttributes.Public);
            }

            typeBuilder.AddInterfaceImplementation(sourceType);

            DefaultConstructorGenerator.CreateDefaultConstructors(typeBuilder, parentType);
            
            var typeContext = this.CreateTypeContext(parameters, typeBuilder);

            var result = this.ImplementInterfaceMethods(typeContext, sourceType.GetAllInterfaces(), parentType);

            var generatedType = typeBuilder.CreateTypeInfo();

            var factory = GenerateFactoryDelegate(sourceType, generatedType);

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
            TTypeContext typeContext,
            Type @interface,
            TypeBuilder typeBuilder,
            MethodInfo sourceMethodInfo,
            ParameterInfo[] parameters,
            Type[] genericArguments)
        {
            return default(TMethodContext);
        }

        private ImmutableList<string> CreateMethod(
            TTypeContext typeContext,
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

                var methodContext = this.CreateMethodContext(typeContext, @interface, typeBuilder, sourceMethod, parameters, genericArguments);

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

        private ImplementInterfaceMethodResult ImplementInterfaceMethods(TTypeContext typeContext, IEnumerable<Type> interfaces, Type parentType)
        {
            var result = ImplementInterfaceMethodResult.Empty;

            foreach (var interfaceType in interfaces)
            {
                result = result.AddUsedNames(
                    this.CreateMethod(
                        typeContext,
                        typeContext.TypeBuilder,
                        interfaceType,
                        result.NamesUsed,
                        parentType));

                result = result.AddImplementedInterface(interfaceType);
            }

            return result;
        }
    }

    public class BaseTypeCloneTypeContext
    {
        public GenerateTypeParameters Parameters { get; }

        public TypeBuilder TypeBuilder { get; }

        public BaseTypeCloneTypeContext(GenerateTypeParameters parameters, TypeBuilder typeBuilder)
        {
            this.Parameters = parameters;
            this.TypeBuilder = typeBuilder;
        }
    }
}