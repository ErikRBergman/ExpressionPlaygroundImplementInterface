namespace Serpent.InterfaceProxy.NetFramework.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        [TestMethod]
        public void CreateModifiedInterfaceTest()
        {
            var parameters = TypeCloneBuilderParameters<TypeContext, MethodContext>
                    .New
                    .AddInterface(typeof(ITestInterface))
                    .TypeName(typeof(ITestInterface).FullName + "_Clone")
                    .OutputInterface();

            var typeCloneBuilder = new TypeCloneBuilder<TypeContext, MethodContext>();
            var generatedType = typeCloneBuilder.GenerateType(parameters);

            Assert.IsTrue(generatedType.GeneratedType.IsInterface);
        }

    }
}