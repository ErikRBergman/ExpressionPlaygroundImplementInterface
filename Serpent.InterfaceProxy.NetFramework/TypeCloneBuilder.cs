// ReSharper disable StyleCop.SA1402
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

    public class TypeCloneBuilder<TTypeContext, TMethodContext>
        where TTypeContext : BaseTypeContext, new()
        where TMethodContext : BaseMethodContext, new()
    {
        public TypeCloneBuilder()
        {
            this.ModuleBuilder = DefaultValues.DefaultModuleBuilder;
        }

        public ModuleBuilder ModuleBuilder { get; set; }
        
        public GenerateTypeResult GenerateType(TypeCloneBuilderParameters parameters)
        {
            var areParametersValid = parameters.IsValid();
            if (areParametersValid.IsValid == false)
            {
                throw new ArgumentException(string.Join(", ", areParametersValid.Errors));
            }

            var parentType = parameters.ParentType;

            var typeName = parameters.TypeName;

            var newTypeAttributes = parameters.TypeAttributes;
            var typeBuilder = this.DefineType(typeName, newTypeAttributes, parentType);

            foreach (var @interface in parameters.InterfacesToImplement)
            {
                typeBuilder.AddInterfaceImplementation(@interface);
            }

            if (parentType != null)
            {
                DefaultConstructorGenerator.CreateDefaultConstructors(typeBuilder, parentType);
            }

            var interfaces = parameters.InterfacesToImplement.SelectMany(type => type.GetAllInterfaces()).ToArray();

            var typeContext = new TTypeContext
                                  {
                                      TypeBuilder = typeBuilder,
                                      Parameters = parameters
                                  };

            var createMethodsResult = this.CreateMethods(typeContext, interfaces, parentType);

            // Generate the type
            var generatedType = typeBuilder.CreateTypeInfo();

            var factories = interfaces.Select(i => GenerateFactoryDelegate(i, generatedType));

            return new GenerateTypeResult(generatedType, createMethodsResult.InterfacesImplemented, factories);
        }

        private TypeBuilder DefineType(string typeName, TypeAttributes newTypeAttributes, Type parentType)
        {
            TypeBuilder typeBuilder;
            if (parentType != null)
            {
                typeBuilder = this.ModuleBuilder.DefineType(typeName, newTypeAttributes, parentType);
            }
            else
            {
                typeBuilder = this.ModuleBuilder.DefineType(typeName, newTypeAttributes);
            }

            return typeBuilder;
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
            TTypeContext typeContext,
            MethodInfo sourceMethodInfo,
            ParameterInfo[] sourceMethodParameters,
            Type[] genericArguments,
            string[] genericArgumentNames)
        {
            var typeBuilder = typeContext.TypeBuilder;

            Func<MethodInfo, Type, IEnumerable<TypeBuilderMethodParameter>, IEnumerable<TypeBuilderMethodParameter>> parameterFunc = typeContext.Parameters.MethodModifierFunc ?? ((MethodInfo m, Type t, IEnumerable<TypeBuilderMethodParameter> ps) => ps);

            var newParameters = parameterFunc(sourceMethodInfo, sourceMethodInfo.ReturnType, sourceMethodParameters.Select(p => new TypeBuilderMethodParameter(p))).ToArray();

            var interfaceImplementationMethodBuilder = typeBuilder.DefineMethod(
                sourceMethodInfo.Name,
                typeContext.Parameters.MethodAttributes,
                sourceMethodInfo.ReturnType,
                newParameters.Select(pi => pi.ParameterType).ToArray());

            if (genericArguments.Any())
            {
                var genericParameters = interfaceImplementationMethodBuilder.DefineGenericParameters(genericArgumentNames);
                var substituteTypes = genericArguments.ZipToDictionaryMap(genericParameters);

                var returnType = sourceMethodInfo.ReturnType;

                var newReturnType = substituteTypes.GetSubstitute(returnType);

                interfaceImplementationMethodBuilder.SetReturnType(newReturnType);
            }

            UpdateParameterAttributes(interfaceImplementationMethodBuilder, newParameters);

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

        private static void UpdateParameterAttributes(MethodBuilder interfaceImplementationMethodBuilder, TypeBuilderMethodParameter[] parameters)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                var builder = interfaceImplementationMethodBuilder.DefineParameter(i + 1, parameters[i].Attributes, parameters[i].Name);

                if (parameters[i].HasParamArrayArgument)
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

        private ImmutableList<string> CreateInterfaceMethods(
            TTypeContext typeContext,
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

                var methodContext = this.CreateMethodContext(@interface, typeContext.TypeBuilder, sourceMethod, parameters, genericArguments);

                var interfaceImplementationMethodBuilder = CreateMethod(
                    typeContext,
                    sourceMethod,
                    parameters,
                    genericArguments,
                    genericArgumentNames);

                this.EmitMethodImplementation(@interface, sourceMethod, interfaceImplementationMethodBuilder, methodContext, parentType);
            }

            return usedNames;
        }

        private ImplementInterfaceMethodResult CreateMethods(TTypeContext typeContext, IEnumerable<Type> interfaces, Type parentType)
        {
            var result = ImplementInterfaceMethodResult.Empty;

            foreach (var interfaceType in interfaces)
            {
                result = result.AddUsedNames(
                    this.CreateInterfaceMethods(
                        typeContext,
                        interfaceType,
                        result.NamesUsed,
                        parentType));

                result = result.AddImplementedInterface(interfaceType);
            }

            return result;
        }
    }

    public class TypeCloneBuilder : TypeCloneBuilder<TypeCloneBuilder.TypeCloneBuilderTypeContext, TypeCloneBuilder.TypeCloneBuilderMethodContext>
    {
        public class TypeCloneBuilderMethodContext : BaseMethodContext
        {

        }

        public class TypeCloneBuilderTypeContext : BaseTypeContext
        {

        }
    }
}