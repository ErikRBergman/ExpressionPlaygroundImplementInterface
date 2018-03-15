namespace Serpent.InterfaceProxy.NetFramework.Tests
{
    using System;
    using System.Collections.Immutable;
    using System.Reflection;
    using System.Reflection.Emit;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Serpent.InterfaceProxy.Extensions;
    using Serpent.InterfaceProxy.NetFramework.Tests.Test;
    using Serpent.InterfaceProxy.NetFramework.Tests.Test.Interfaces;

    [TestClass]
    public class TypeCloneBuilderTests
    {
        private class MethodContext : BaseMethodContext
        {

        }

        private class TypeContext : BaseTypeContext<TypeContext, MethodContext>
        {

        }

        private struct MyContext
        {
            public string RequestId { get; set; }

            public string CorrelationId { get; set; }
        }

        [TestMethod]
        public void CreateModifiedInterfaceTest()
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("Serpent.ProxyBuilder.GeneratedTypes"),
                AssemblyBuilderAccess.RunAndSave);

            var module = assembly.DefineDynamicModule(assembly.GetName().Name, assembly.GetName().Name + ".dll");

            var parameters = new TypeCloneBuilderParameters<TypeContext, MethodContext>()
            {
                CreateMethodFunc = (data, context) =>
                    {
                        data.Parameters = ImmutableList<TypeBuilderMethodParameter>.Empty.Add(
                                new TypeBuilderMethodParameter
                                {
                                    Name = "NEW_PARAMETER",
                                    ParameterType = typeof(MyContext),
                                })
                            .AddRange(data.Parameters);

                        return new CreateMethodFuncResult<MethodContext>(data, null);
                    }
            }
            .AddInterface(typeof(ITestInterface))
            .TypeName(typeof(ITestInterface).FullName + "_Clone")
            .ModuleBuilder(module)
            .OutputInterface();

            var typeCloneBuilder = new TypeCloneBuilder<TypeContext, MethodContext>();
            var generatedType = typeCloneBuilder.GenerateType(parameters);

            Assert.IsTrue(generatedType.GeneratedType.IsInterface);

            assembly.Save(assembly.GetName().Name + ".dll");
        }

    }
}