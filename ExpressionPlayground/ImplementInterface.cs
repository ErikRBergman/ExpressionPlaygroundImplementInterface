using System;

namespace ExpressionPlayground
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Sigil;

    using Xunit;

    public class ImplementInterface
    {
        private AssemblyName assemblyName;

        private AssemblyBuilder assemblyBuilder;

        private ModuleBuilder moduleBuilder;

        private MethodInfo getTypeFromHandleMethodInfo;



        public ImplementInterface()
        {

            var domain = AppDomain.CurrentDomain;
#if DEBUG
            this.assemblyName = new AssemblyName("Serpent.ProxyBuilder.Debug");
#else
            string assemblyUniqueName = Guid.NewGuid().ToString("N");
            this.assemblyName = new AssemblyName("serpent.ProxyBuilder_" + assemblyUniqueName);
#endif
            this.assemblyBuilder = domain.DefineDynamicAssembly(this.assemblyName, AssemblyBuilderAccess.RunAndSave);
            this.moduleBuilder = this.assemblyBuilder.DefineDynamicModule(this.assemblyName.Name, this.assemblyName.Name + ".dll");
        }

        [Fact]
        public void GenerateProxy()
        {
            var interfaceToImplement = typeof(IInterfaceToImplement);

            var baseClass = typeof(ProxyBase<IInterfaceToImplement>);

            var typeName = interfaceToImplement + "+serviceInterfaceProxy";
            TypeBuilder typeBuilder = this.moduleBuilder.DefineType(typeName, TypeAttributes.Public);
            typeBuilder.SetParent(baseClass);
            typeBuilder.AddInterfaceImplementation(interfaceToImplement);

            // TODO: Add the outer class parameter to the constructor and add code to store it
            this.CreateSuperClassConstructorCalls(typeBuilder, baseClass);

            var result = this.ImplementInterfaceProxy(interfaceToImplement, typeBuilder, baseClass);

            var newType = typeBuilder.CreateType();

            var dynamicType = (IInterfaceToImplement)Activator.CreateInstance(newType, new OurImplementation());

            var model = dynamicType.GetModel(123, "123");

            this.assemblyBuilder.Save(this.assemblyName.Name + ".dll");

        }

        private DynamicImplementInterfaceResult ImplementInterfaceProxy(Type interfaceType, TypeBuilder typeBuilder, Type baseClass, DynamicImplementInterfaceResult? previousResult = null)
        {
            var result = previousResult ?? DynamicImplementInterfaceResult.Empty;
            var innerInstanceFieldInfo = baseClass.GetField("inner", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (interfaceType != typeof(IDisposable))
            {
                result = result.AddUsedNames(this.GenerateMethods(result.NamesUsed, interfaceType, typeBuilder, innerInstanceFieldInfo));

                foreach (Type i in interfaceType.GetInterfaces())
                {
                    if (!result.InterfacesImplemented.Contains(i))
                    {
                        var innerResult = this.ImplementInterfaceProxy(i, typeBuilder, baseClass, result);
                        result = result.Add(innerResult).AddImplementedInterface(i);
                    }
                }
            }

            return result;
        }

        private ImmutableList<string> GenerateMethods(ImmutableList<string> usedNames, Type interfaceType, TypeBuilder typeBuilder, FieldInfo innerInstanceFieldInfo)
        {
            foreach (MethodInfo methodInfo in interfaceType.GetMethods())
            {
                var parameterInfoArray = methodInfo.GetParameters();
                var genericArgumentArray = methodInfo.GetGenericArguments();

                string paramNames = string.Join(", ", parameterInfoArray.Select(pi => pi.ParameterType));
                string nameWithParams = string.Concat(methodInfo.Name, "(", paramNames, ")");
                if (usedNames.Contains(nameWithParams))
                {
                    throw new NotSupportedException(
                        string.Format("Error in interface {1}! Method '{0}' already used in other child interface!", nameWithParams, interfaceType.Name)); //LOCSTR
                }

                usedNames = usedNames.Add(nameWithParams);

                var genericArgumentNames = genericArgumentArray.Select(pi => pi.Name).ToArray();

                var methodBuilder = typeBuilder.DefineMethod(
                    methodInfo.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    methodInfo.ReturnType,
                    parameterInfoArray.Select(pi => pi.ParameterType).ToArray());
                if (genericArgumentArray.Any())
                {
                    methodBuilder.DefineGenericParameters(genericArgumentNames);
                }

                if (parameterInfoArray.Length > 0)
                {
                    var closureType = CreateClosureType(typeBuilder, methodInfo, genericArgumentArray, genericArgumentNames, parameterInfoArray);
                    this.EmitMethodImplementationWithParameters(methodInfo, methodBuilder, innerInstanceFieldInfo, closureType);
                }
                else
                {
                    this.EmitMethodImplementationWithoutParameters(methodInfo, methodBuilder, innerInstanceFieldInfo);
                }

                // Since we're implementing an interface
                typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
            }

            return usedNames;
        }

        private static Type CreateClosureType(
            TypeBuilder typeBuilder,
            MethodInfo methodInfo,
            Type[] genericArgumentArray,
            string[] genericArgumentNames,
            ParameterInfo[] parameterInfoArray)
        {
            var closureTypeBuilder = typeBuilder.DefineNestedType(methodInfo.Name + "_" + Guid.NewGuid().ToString("N"), TypeAttributes.NestedPrivate | TypeAttributes.Sealed);

            var closureGenericArguments = genericArgumentArray.Length == 0 ? Array.Empty<GenericTypeParameterBuilder>() : closureTypeBuilder.DefineGenericParameters(genericArgumentNames);

            foreach (var parameterInfo in parameterInfoArray)
            {
                var parameterType = parameterInfo.ParameterType;

                var index = Array.IndexOf(genericArgumentArray, parameterType);
                if (index != -1)
                {
                    parameterType = closureGenericArguments[index];
                }

                closureTypeBuilder.DefineField(parameterInfo.Name, parameterType, FieldAttributes.Public);
            }

            var closureType = closureTypeBuilder.CreateType();
            return closureType;
        }

        private void EmitMethodImplementationWithoutParameters(MethodInfo mi, MethodBuilder mb, FieldInfo innerInstanceFieldInfo)
        {
            var generator = mb.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0); // this
            generator.Emit(OpCodes.Ldfld, innerInstanceFieldInfo); // .inner

            var parameters = mi.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                generator.Emit(OpCodes.Ldarg, i + 1); // parameter x
            }

            generator.EmitCall(OpCodes.Callvirt, mi, null); // call the inner method
            generator.Emit(OpCodes.Ret);
        }

        private void EmitMethodImplementationWithParameters(MethodInfo mi, MethodBuilder mb, FieldInfo innerInstanceFieldInfo, Type closureType)
        {
            var generator = mb.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0); // this
            generator.Emit(OpCodes.Ldfld, innerInstanceFieldInfo); // .inner

            var parameters = mi.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                generator.Emit(OpCodes.Ldarg, i + 1); // parameter x
            }

            generator.EmitCall(OpCodes.Callvirt, mi, null); // call the inner method
            generator.Emit(OpCodes.Ret);
        }

        private void CreateSuperClassConstructorCalls(TypeBuilder typeBuilder, Type baseClass)
        {
            foreach (var baseConstructor in baseClass.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var parameters = baseConstructor.GetParameters();
                if (parameters.Length > 0 && parameters.Last().IsDefined(typeof(ParamArrayAttribute), false))
                {
                    throw new InvalidOperationException("Variadic constructors are not supported"); //LOCSTR
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
            }
        }


    }
}