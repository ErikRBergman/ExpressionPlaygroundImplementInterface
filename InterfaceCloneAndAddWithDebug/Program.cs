using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using InterfaceCloneAndAddWithDebug.Interfaces;

namespace InterfaceCloneAndAddWithDebug
{

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class MyAttribute : Attribute
    {
        public String s;
        public int x;

        public MyAttribute(String s, int x)
        {
            this.s = s;
            this.x = x;
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var type = typeof(IMyGenericInterface<>);
            var interfaces = type.GetActorInterfaces(true);

            Console.WriteLine("Generating interface clones for:");

            var assemblyName = "_______serpent_type_clone_" + type.Assembly.GetName().Name + "_______serpent_type_clone_c52eae52-af5f-4643-9ed7-39dfd2523201";
            var moduleName = assemblyName;
            var saveOnDisk = true;

            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
#if !DotNetCoreClr
                new AssemblyName(assemblyName),
                saveOnDisk ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.RunAndCollect
#else
                new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndCollect
#endif
            );

            var moduleFilename = moduleName + ".dll";

            var moduleBuilder =
#if !DotNetCoreClr
                saveOnDisk
                    ? assemblyBuilder.DefineDynamicModule(moduleName, moduleFilename, true)
                    : assemblyBuilder.DefineDynamicModule(moduleName);
#else
            assemblyBuilder.DefineDynamicModule(moduleName);
#endif

            var infoConstructor = typeof(MyAttribute).GetConstructor(new Type[2] { typeof(string), typeof(int) });
            CustomAttributeBuilder attributeBuilder =
                new CustomAttributeBuilder(infoConstructor, new object[2] { "Hello", 2 });
            assemblyBuilder.SetCustomAttribute(attributeBuilder);


            var oldTypeToNewTypeMap = new Dictionary<Type, Type>();

            foreach (var @interface in interfaces.Reverse())
            {
                var interfaceFullName = @interface.FullName ?? @interface.Namespace + "." + @interface.Name;

                Console.WriteLine($" * {interfaceFullName}");

                var newInterfaceTypeBuilder = moduleBuilder.DefineType(interfaceFullName, TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public);

                // Create generic arguments, and create a map to get the generic arguments later when we need them
                var genericTypeParametersMap = CloneGenericParameters(@interface, newInterfaceTypeBuilder);

                newInterfaceTypeBuilder.AddInterfaceImplementations(@interface.GetActorInterfaces(), t => oldTypeToNewTypeMap.GetValueOrDefault(t, t));

                CreateMethods(newInterfaceTypeBuilder, @interface, genericTypeParametersMap);

                var newInterface = newInterfaceTypeBuilder.CreateTypeInfo().AsType();

                oldTypeToNewTypeMap[@interface] = newInterface;
            }

            assemblyBuilder.Save(assemblyBuilder.GetName().Name + ".dll");

            Console.ReadKey();
        }

        private static void CreateMethods(TypeBuilder newInterfaceTypeBuilder, Type @interface, IReadOnlyDictionary<Type, GenericTypeParameterBuilder> genericTypeParametersMap)
        {
            var methods = @interface.GetMethods();

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();

                var genericArguments = method.GetGenericArguments();
                var genericArgumentNames = genericArguments.Select(pi => pi.Name).ToArray();

                // Substitute generic parameters from the interface with the ones in the new interface
                var parameterTypes = parameters.Select(
                    p =>
                        {
                            if (genericTypeParametersMap.TryGetValue(p.ParameterType, out var newType))
                            {
                                return newType;
                            }

                            return p.ParameterType;
                        });


                var newMethod = newInterfaceTypeBuilder.DefineMethod(
                    method.Name,
                    method.Attributes,
                    method.CallingConvention,
                    method.ReturnType,
                    parameterTypes.ToArray());

                for (var i = 0; i < parameters.Length; i++)
                {
                    var builder = newMethod.DefineParameter(i + 1, parameters[i].Attributes, parameters[i].Name);

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
        }

        private static IReadOnlyDictionary<Type, GenericTypeParameterBuilder> CloneGenericParameters(Type @interface, TypeBuilder newInterfaceTypeBuilder)
        {
            var genericParameters = @interface.GetGenericArguments();
            var genericParametersMap = new Dictionary<Type, GenericTypeParameterBuilder>();

            if (genericParameters.Length > 0)
            {
                var genericParametersBuilder = newInterfaceTypeBuilder.DefineGenericParameters(genericParameters.Select(g => g.Name).ToArray());

                foreach (var param in genericParameters.Zip(genericParametersBuilder, (t, gpb) => new KeyValuePair<Type, GenericTypeParameterBuilder>(t, gpb)))
                {
                    genericParametersMap[param.Key] = param.Value;
                }
            }

            return genericParametersMap;
        }
    }
}