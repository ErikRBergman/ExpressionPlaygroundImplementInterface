namespace Serpent.IntermediateLanguageTools.Tests
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Serpent.IntermediateLanguageTools.Helpers;

    [TestClass]
    public class TypeExtensionsTests
    {
        [TestMethod]
        public void GetCSharpNameTests()
        {
            Assert.AreEqual("Int32", typeof(int).GetCSharpName());
            Assert.AreEqual("System.Int32", typeof(int).GetCSharpName(true));

            Assert.AreEqual("KeyValuePair<Int32, String>", typeof(KeyValuePair<int, string>).GetCSharpName());
            Assert.AreEqual("System.Collections.Generic.KeyValuePair<System.Int32, System.String>", typeof(KeyValuePair<int, string>).GetCSharpName(true));

            Assert.AreEqual("KeyValuePair<Int32, KeyValuePair<Int32, String>>", typeof(KeyValuePair<int, KeyValuePair<int, string>>).GetCSharpName());
            Assert.AreEqual("System.Collections.Generic.KeyValuePair<System.Int32, System.Collections.Generic.KeyValuePair<System.Int32, System.String>>", typeof(KeyValuePair<int, KeyValuePair<int, string>>).GetCSharpName(true));

            Assert.AreEqual("KeyValuePair<,>", typeof(KeyValuePair<,>).GetCSharpName());
            Assert.AreEqual("System.Collections.Generic.KeyValuePair<,>", typeof(KeyValuePair<,>).GetCSharpName(true));
        }
    }
}