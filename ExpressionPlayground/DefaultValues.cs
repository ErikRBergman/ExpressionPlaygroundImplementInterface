namespace ExpressionPlayground
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    public static class DefaultValues
    {
        static DefaultValues()
        {
            var assemblyName = DefaultAssemblyBuilder.GetName().Name;
            DefaultModuleBuilder = DefaultAssemblyBuilder.DefineDynamicModule(assemblyName, assemblyName + ".dll");
        }

        public static AssemblyBuilder DefaultAssemblyBuilder { get; } = AppDomain.CurrentDomain.DefineDynamicAssembly(
            new AssemblyName("Serpent.ProxyBuilder.GeneratedTypes"),
            AssemblyBuilderAccess.RunAndSave);

        public static ModuleBuilder DefaultModuleBuilder { get; }
    }
}