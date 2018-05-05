// ReSharper disable StyleCop.SA1402

namespace Serpent.InterfaceProxy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Serpent.InterfaceProxy.Extensions;
    using Serpent.InterfaceProxy.Helpers;
    using Serpent.InterfaceProxy.ImplementationBuilders;
    using Serpent.InterfaceProxy.Types;

    public class TypeCloneBuilder<TTypeContext, TMethodContext>
        where TTypeContext : BaseTypeContext<TTypeContext, TMethodContext>, new()
        where TMethodContext : BaseMethodContext, new()
    {
        public virtual GenerateTypeResult GenerateType(TypeCloneBuilderParameters<TTypeContext, TMethodContext> parameters)
        {
            var areParametersValid = parameters.IsValid();
            if (areParametersValid.IsValid == false)
            {
                throw new ArgumentException(string.Join(", ", areParametersValid.Errors));
            }

            var parentType = parameters.ParentType;

            var typeBuilder = this.DefineType(parameters.ModuleBuilder, parameters.TypeName, parameters.TypeAttributes, parentType);

            // Invoke event
            parameters.OnTypeBuilderCreatedAction?.Invoke(typeBuilder);

            // Add interfaces to implement
            foreach (var @interface in parameters.InterfacesToImplement)
            {
                typeBuilder.AddInterfaceImplementation(@interface);
            }

            // Create default constructors
            if (parentType != null)
            {
                DefaultConstructorGenerator.CreateDefaultConstructors(typeBuilder, parentType);
            }

            // Get all unique interfaces to implement
            var interfaces = parameters.InterfacesToImplement.SelectMany(type => type.GetAllInterfaces()).Distinct().ToArray();

            // Invoke event
            parameters.OnTypeBuilderCreatedAndConfiguredAction?.Invoke(typeBuilder);

            // Create methods
            var createMethodsResult = this.CreateMethods(typeBuilder, parameters, interfaces, parentType);

            // Invoke event
            parameters.OnBeforeTypeIsFinalizedAction?.Invoke(typeBuilder);

            // Generate the new type
            var generatedType = typeBuilder.CreateTypeInfo();

            // Create a factory for each interface
            var factories = GenerateFactoryDelegates(generatedType);

            return new GenerateTypeResult(generatedType, createMethodsResult.InterfacesImplemented, factories);
        }

        protected virtual void EmitMethodImplementation(Type interfaceType, MethodInfo sourceMethodInfo, MethodBuilder methodBuilder, TMethodContext context, Type parentType)
        {
        }

        private static MethodBuilder CreateMethod(TTypeContext typeContext, CreateMethodFuncResult<TMethodContext> createMethodContext)
        {
            var typeBuilder = typeContext.TypeBuilder;

            var sourceMethodInfo = createMethodContext.CreateMethodData.SourceMethodInfo;

            var interfaceImplementationMethodBuilder = typeBuilder.DefineMethod(
                sourceMethodInfo.Name,
                typeContext.Parameters.MethodAttributes,
                sourceMethodInfo.ReturnType,
                createMethodContext.CreateMethodData.Parameters.Select(pi => pi.ParameterType).ToArray());

            var genericArguments = createMethodContext.CreateMethodData.GenericArguments;
            var genericArgumentNames = createMethodContext.CreateMethodData.GenericArgumentNames;

            if (genericArguments.Any())
            {
                var genericParameters = interfaceImplementationMethodBuilder.DefineGenericParameters(genericArgumentNames);
                var substituteTypes = genericArguments.ZipToDictionaryMap(genericParameters);

                var returnType = sourceMethodInfo.ReturnType;

                var newReturnType = substituteTypes.GetSubstitute(returnType);

                interfaceImplementationMethodBuilder.SetReturnType(newReturnType);
            }

            UpdateParameterAttributes(interfaceImplementationMethodBuilder, createMethodContext.CreateMethodData.Parameters.ToArray());

            return interfaceImplementationMethodBuilder;
        }

        private static IEnumerable<Delegate> GenerateFactoryDelegates(Type generatedType)
        {
            var constructors = generatedType.GetConstructors();
            var factoryTypes = generatedType.GetInterfaces().Prepend(typeof(object)).Prepend(generatedType).Prepend(generatedType.BaseType).ToArray();

            foreach (var factoryType in factoryTypes)
            {
                // create a factory for each public constructor
                foreach (var constructor in constructors)
                {
                    var parameters = constructor.GetParameters();

                    var builder = new DynamicMethod("TypeFactory_" + generatedType.Name, factoryType, parameters.Select(p => p.ParameterType).ToArray());
                    var generator = builder.GetILGenerator();

                    for (var i = 0; i < parameters.Length; i++)
                    {
                        generator.Emit(OpCodes.Ldarg_S, i);
                    }

                    generator.Emit(OpCodes.Newobj, constructor);
                    generator.Emit(OpCodes.Ret);

                    yield return builder.CreateDelegate(FuncHelper.Create(factoryType, parameters.Select(p => p.ParameterType)));
                }
            }
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

                if (parameters[i].Attributes.HasFlag(ParameterAttributes.HasDefault) && parameters[i].DefaultValue != null)
                {
                    builder.SetConstant(parameters[i].DefaultValue);
                }
            }
        }

        private ImmutableList<string> CreateInterfaceMethods(TTypeContext typeContext, Type @interface, ImmutableList<string> usedNames)
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

                var createMethodContextFunc =
                    typeContext.Parameters.CreateMethodFunc ?? ((data, context) => new CreateMethodFuncResult<TMethodContext>(data, new TMethodContext()));

                var createMethodData = new CreateMethodData
                                           {
                                               SourceMethodInfo = sourceMethod,
                                               TypeBuilder = typeContext.TypeBuilder,
                                               SourceType = typeContext.SourceType,
                                               GenericArguments = genericArguments,
                                               GenericArgumentNames = genericArgumentNames,
                                               Parameters = parameters.Select(p => new TypeBuilderMethodParameter(p))
                                           };

                var createMethodContext = createMethodContextFunc(createMethodData, typeContext);

                var interfaceImplementationMethodBuilder = CreateMethod(typeContext, createMethodContext);

                this.EmitMethodImplementation(@interface, sourceMethod, interfaceImplementationMethodBuilder, createMethodContext.MethodContext, typeContext.ParentType);
            }

            return usedNames;
        }

        private ImplementInterfaceMethodResult CreateMethods(
            TypeBuilder typeBuilder,
            TypeCloneBuilderParameters<TTypeContext, TMethodContext> parameters,
            IEnumerable<Type> interfaces,
            Type parentType)
        {
            var result = ImplementInterfaceMethodResult.Empty;

            foreach (var interfaceType in interfaces)
            {
                var typeContext = new TTypeContext
                                      {
                                          SourceType = interfaceType,
                                          TypeBuilder = typeBuilder,
                                          Parameters = parameters,
                                          ParentType = parentType
                                      };

                var methodNames = this.CreateInterfaceMethods(typeContext, interfaceType, result.NamesUsed);

                result = result.AddUsedNames(methodNames);

                result = result.AddImplementedInterface(interfaceType);
            }

            return result;
        }

        private TypeBuilder DefineType(ModuleBuilder moduleBuilder, string typeName, TypeAttributes newTypeAttributes, Type parentType)
        {
            TypeBuilder typeBuilder;
            if (parentType != null)
            {
                typeBuilder = moduleBuilder.DefineType(typeName, newTypeAttributes, parentType);
            }
            else
            {
                typeBuilder = moduleBuilder.DefineType(typeName, newTypeAttributes);
            }

            return typeBuilder;
        }
    }

    public class TypeCloneBuilder : TypeCloneBuilder<TypeCloneBuilder.TypeCloneBuilderTypeContext, TypeCloneBuilder.TypeCloneBuilderMethodContext>
    {
        public class TypeCloneBuilderMethodContext : BaseMethodContext
        {
        }

        public class TypeCloneBuilderTypeContext : BaseTypeContext<TypeCloneBuilderTypeContext, TypeCloneBuilderMethodContext>
        {
        }
    }
}