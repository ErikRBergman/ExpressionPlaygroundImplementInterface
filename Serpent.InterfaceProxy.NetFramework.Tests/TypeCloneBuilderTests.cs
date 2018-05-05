namespace Serpent.InterfaceProxy.NetFramework.Tests
{
    using System.Collections.Immutable;
    using System.Reflection;
    using System.Reflection.Emit;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Serpent.InterfaceProxy.Core;
    using Serpent.InterfaceProxy.Extensions;
    using Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder;
    using Serpent.InterfaceProxy.NetFramework.Tests.Test.Interfaces;

    [TestClass]
    public class TypeCloneBuilderTests
    {
        [TestMethod]
        public void CreateModifiedInterfaceTest()
        {
            //var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Serpent.ProxyBuilder.GeneratedTypes"), AssemblyBuilderAccess.RunAndSave);
            //var module = assembly.DefineDynamicModule(assembly.GetName().Name, assembly.GetName().Name + ".dll");

            var module = DefaultValues.DefaultModuleBuilder;

            var parameters = new TypeCloneBuilderParameters<TypeContext, MethodContext>
                                 {
                                     CreateMethodFunc = (data, context) =>
                                         {
                                             data.Parameters = ImmutableList<TypeBuilderMethodParameter>.Empty.Add(
                                                     new TypeBuilderMethodParameter
                                                         {
                                                             Name = "NEW_PARAMETER",
                                                             ParameterType = typeof(MyContext)
                                                         })
                                                 .AddRange(data.Parameters);

                                             return new CreateMethodFuncResult<MethodContext>(data, null);
                                         }
                                 }.AddInterface(typeof(ITestInterface))
                .TypeName(typeof(ITestInterface).FullName + "_Clone")
                .ModuleBuilder(module)
                .OutputInterface();

            var typeCloneBuilder = new TypeCloneBuilder<TypeContext, MethodContext>();
            var generatedType = typeCloneBuilder.GenerateType(parameters);

            Assert.IsTrue(generatedType.GeneratedType.IsInterface);

            // assembly.Save(assembly.GetName().Name + ".dll");
        }


        [TestMethod]
        public void CreateProxyFromCreatedModifiedInterfaceTest()
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Serpent.ProxyBuilder.GeneratedTypes"), AssemblyBuilderAccess.RunAndSave);

            var module = assembly.DefineDynamicModule(assembly.GetName().Name, assembly.GetName().Name + ".dll");

            var parameters = new TypeCloneBuilderParameters<TypeContext, MethodContext>
                                 {
                                     CreateMethodFunc = (data, context) =>
                                         {
                                             data.Parameters = ImmutableList<TypeBuilderMethodParameter>.Empty.Add(
                                                     new TypeBuilderMethodParameter
                                                         {
                                                             Name = "NEW_PARAMETER",
                                                             ParameterType = typeof(MyContext)
                                                         })
                                                 .AddRange(data.Parameters);

                                             return new CreateMethodFuncResult<MethodContext>(data, null);
                                         }
                                 }.AddInterface(typeof(ITestInterface))
                .TypeName(typeof(ITestInterface).FullName)
                .ModuleBuilder(module)
                .OutputInterface();

            var typeCloneBuilder = new TypeCloneBuilder<TypeContext, MethodContext>();
            var generatedType = typeCloneBuilder.GenerateType(parameters);

            Assert.IsTrue(generatedType.GeneratedType.IsInterface);

            var proxyBuilder = new ProxyTypeBuilder();




            assembly.Save(assembly.GetName().Name + ".dll");
        }

        private struct MyContext
        {
            public string CorrelationId { get; set; }

            public string RequestId { get; set; }
        }

        private class MethodContext : BaseMethodContext
        {
        }

        private class TypeContext : BaseTypeContext<TypeContext, MethodContext>
        {
        }
    }
}