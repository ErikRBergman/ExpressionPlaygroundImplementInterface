namespace Serpent.InterfaceProxy
{
    using System.Reflection;
    using System.Reflection.Emit;

    using Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder;

    public static class DefaultValues
    {
        static DefaultValues()
        {
            var assemblyName = DefaultAssemblyBuilder.GetName().Name;
#if NETSTANDARD2_0
            DefaultModuleBuilder = DefaultAssemblyBuilder.DefineDynamicModule(assemblyName);
#else
            DefaultModuleBuilder = DefaultAssemblyBuilder.DefineDynamicModule(assemblyName, assemblyName + ".dll");
#endif
        }

#if NETSTANDARD2_0
        public static AssemblyBuilder DefaultAssemblyBuilder { get; } = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName("Serpent.ProxyBuilder.GeneratedTypes"),
            AssemblyBuilderAccess.Run);
#else
        public static AssemblyBuilder DefaultAssemblyBuilder { get; } = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName("Serpent.ProxyBuilder.GeneratedTypes"),
            AssemblyBuilderAccess.RunAndSave);
#endif

        public static ModuleBuilder DefaultModuleBuilder { get; }

        public static string DefaultTypeNamespace { get; } = typeof(ProxyTypeBuilder).Namespace + ".GeneratedTypes";
    }
}