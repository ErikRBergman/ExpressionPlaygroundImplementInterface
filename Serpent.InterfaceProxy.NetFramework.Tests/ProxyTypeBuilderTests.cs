// ReSharper disable UnusedVariable
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Serpent.InterfaceProxy.NetFramework.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Serpent.InterfaceProxy.Extensions;
    using Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder;
    using Serpent.InterfaceProxy.NetFramework.Tests.Test;
    using Serpent.InterfaceProxy.NetFramework.Tests.Test.Implementation;
    using Serpent.InterfaceProxy.NetFramework.Tests.Test.Interfaces;
    using Serpent.InterfaceProxy.Validation;

    [TestClass]
    public class ProxyTypeBuilderTests
    {
        private static readonly object proxyLock = new object();

        private static GenerateProxyResult<ITestInterface> proxy;

        [TestMethod]
        public void ComplexGenericStructure_Test()
        {
            var complexGenericStructureObject =
                new KeyValuePair<KeyValuePair<int, KeyValuePair<string, GenericStruct<int>>>, KeyValuePair<string, KeyValuePair<GenericStruct<int>, string>>>(
                    new KeyValuePair<int, KeyValuePair<string, GenericStruct<int>>>(55, new KeyValuePair<string, GenericStruct<int>>("fivefive", new GenericStruct<int>(1, 2))),
                    new KeyValuePair<string, KeyValuePair<GenericStruct<int>, string>>(
                        "two",
                        new KeyValuePair<GenericStruct<int>, string>(new GenericStruct<int>(5, 6), "fivesix")));

            var p = this.GetProxyAndTest();
            var complexGenericStructure = p.Proxy.ComplexGenericStructure(complexGenericStructureObject);
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.ComplexGenericStructure), mc.MethodName) == 0);

            if (complexGenericStructure.Key.Key != 55)
            {
                throw new Exception("Bummer!");
            }
        }

        [TestMethod]
        public async Task GenericResult_GenericParameter_2_Async_Test()
        {
            var p = this.GetProxyAndTest();
            var genericResultGenericParameter = await p.Proxy.GenericResult_GenericParameter_Async(new KeyValuePair<string, int>("One", 1));
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.GenericResult_GenericParameter_Async), mc.MethodName) == 0);
        }

        [TestMethod]
        public async Task GenericResult_GenericParameter_Async_Test()
        {
            var p = this.GetProxyAndTest();
            var resultGenericParameter = await p.Proxy.GenericResult_GenericParameter_Async("C");
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.GenericResult_GenericParameter_Async), mc.MethodName) == 0);
        }

        [TestMethod]
        public void GenericResult_GenericParameter_Test()
        {
            var p = this.GetProxyAndTest();
            var genericResult_GenericParameter = p.Proxy.GenericResult_GenericParameter("uno");
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.GenericResult_GenericParameter), mc.MethodName) == 0);
        }

        [TestMethod]
        public async Task GenericResult_GenericParameters_Async_Test()
        {
            var p = this.GetProxyAndTest();
            var genericResultGenericParameters = await p.Proxy.GenericResult_GenericParameters_Async(1, "Dos", new KeyValuePair<int, string>(3, "Drei"));
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.GenericResult_GenericParameters_Async), mc.MethodName) == 0);
        }

        [TestMethod]
        public void GenericResult_GenericParameters_Test()
        {
            var p = this.GetProxyAndTest();
            var genericResult_GenericParameters = p.Proxy.GenericResult_GenericParameters(1, 0.2m, new ThreeGenericParameter<string, int, float>());
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.GenericResult_GenericParameters), mc.MethodName) == 0);
        }

        [TestMethod]
        public void GenericsAndVarArgs_Test()
        {
            ////void GenericsAndVarArgs<T1>();
            var p = this.GetProxyAndTest();
            var genericsAndVarArgsResult = p.Proxy.GenericsAndVarArgs(1, 2, 3, 4);
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.GenericsAndVarArgs), mc.MethodName) == 0);
        }

        [TestMethod]
        public async Task NoResult_Generic_NoParameters_Async_Test()
        {
            var p = this.GetProxyAndTest();
            await p.Proxy.NoResult_Generic_NoParameters_Async<string>();
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.NoResult_Generic_NoParameters_Async), mc.MethodName) == 0);
        }

        [TestMethod]
        public void NoResult_Generic_NoParameters_Test()
        {
            var p = this.GetProxyAndTest();
            p.Proxy.NoResult_Generic_NoParameters<float>();
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.NoResult_Generic_NoParameters), mc.MethodName) == 0);
        }

        [TestMethod]
        public async Task NoResult_Generic_Parameters_Async_Test()
        {
            var p = this.GetProxyAndTest();
            await p.Proxy.NoResult_Generic_Parameters_Async(1, 2, "haj");
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.NoResult_Generic_Parameters_Async), mc.MethodName) == 0);
        }

        [TestMethod]
        public void NoResult_Generic_Parameters_Test()
        {
            var p = this.GetProxyAndTest();
            p.Proxy.NoResult_Generic_Parameters(1, 0.2m, new ThreeGenericParameter<string, int, float>());
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.NoResult_Generic_Parameters), mc.MethodName) == 0);
        }

        [TestMethod]
        public async Task NoResult_NoParameters_Async_Test()
        {
            var p = this.GetProxyAndTest();
            await p.Proxy.NoResult_NoParameters_Async();
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.NoResult_NoParameters_Async), mc.MethodName) == 0);
        }

        [TestMethod]
        public void NoResult_NoParameters_Test()
        {
            var p = this.GetProxyAndTest();
            p.Proxy.NoResult_NoParameters();
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.NoResult_NoParameters), mc.MethodName) == 0);
        }

        [TestMethod]
        public async Task NoResult_Parameters_Async_Test()
        {
            var p = this.GetProxyAndTest();
            await p.Proxy.NoResult_Parameters_Async(5, "Five");
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.NoResult_Parameters_Async), mc.MethodName) == 0);
        }

        [TestMethod]
        public void NoResult_Parameters_Test()
        {
            var p = this.GetProxyAndTest();
            p.Proxy.NoResult_Parameters(6, "Six");
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.NoResult_Parameters), mc.MethodName) == 0);
        }

        [TestMethod]
        public void Result_Generic_NoParameters_Async_Test()
        {
            var p = this.GetProxyAndTest();
            var result = p.Proxy.Result_Generic_NoParameters_Async<string>();
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.Result_Generic_NoParameters_Async), mc.MethodName) == 0);
        }

        [TestMethod]
        public void Result_Generic_NoParameters_Test()
        {
            var p = this.GetProxyAndTest();
            var result_Generic_NoParameters = p.Proxy.Result_Generic_NoParameters<string>();
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.Result_Generic_NoParameters), mc.MethodName) == 0);
        }

        [TestMethod]
        public async Task Result_Generic_Parameter_Async_Test()
        {
            var p = this.GetProxyAndTest();
            var genericParameter = await p.Proxy.Result_GenericParameter_Async("B");
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.Result_GenericParameter_Async), mc.MethodName) == 0);
            Validator.Default.AreEqual(genericParameter, 123);
        }

        [TestMethod]
        public void Result_GenericParameter_Test()
        {
            var p = this.GetProxyAndTest();
            var result_GenericParameter = p.Proxy.Result_GenericParameter("uno");
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.Result_GenericParameter), mc.MethodName) == 0);
        }

        [TestMethod]
        public async Task Result_NoParameters_Async_Test()
        {
            var p = this.GetProxyAndTest();
            var result_noparameters = await p.Proxy.Result_NoParameters_Async();
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.Result_NoParameters_Async), mc.MethodName) == 0);
        }

        [TestMethod]
        public void Result_NoParameters_Test()
        {
            var p = this.GetProxyAndTest();
            var result_NoParameters = p.Proxy.Result_NoParameters();
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.Result_NoParameters), mc.MethodName) == 0);
        }

        [TestMethod]
        public async Task Result_Parameters_Async_Test()
        {
            var p = this.GetProxyAndTest();
            var resultParameters = await p.Proxy.Result_Parameters_Async(123, "123");
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.Result_Parameters_Async), mc.MethodName) == 0);
            Validator.Default.AreEqual(2, methodCall.Parameters.Length);
            Validator.Default.AreEqual(resultParameters.IntValue, 123);
        }

        [TestMethod]
        public void Result_Parameters_Test()
        {
            var p = this.GetProxyAndTest();
            var result_Parameters = p.Proxy.Result_Parameters(1, "uno");
            var methodCall = p.Test.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(p.Proxy.Result_Parameters), mc.MethodName) == 0);
        }

        [TestMethod]
        public async Task Proxy_With_Multple_Interfaces_Async()
        {
            var proxyResult = CreateProxyTypeWith2Interfaces();

            var factory = proxyResult.GetFactory<IInterface1, IInterface2, ProxyBase<IInterface1, IInterface2>>();

            var c1 = new Class1();
            var c2 = new Class2();
            var instance = factory(c1, c2);

            var i1 = instance as IInterface1;
            var i2 = instance as IInterface2;

            await i1.Method1Async("I1");
            await i2.Method2Async("I2");

            Assert.AreEqual("I1", c1.LastPrefix);
            Assert.AreEqual("I2", c2.LastPrefix);

        }

        private static GenerateProxyResult<ITestInterface> CreateProxyType()
        {
            var parameters = TypeCloneBuilderParameters<ProxyTypeBuilder.TypeContext, ProxyTypeBuilder.MethodContext>.New.AddInterface(typeof(ITestInterface))
                .ParentType(typeof(ProxyBase<>).MakeGenericType(typeof(ITestInterface)))
                .TypeName(typeof(ITestInterface).FullName + "_" + Guid.NewGuid().ToString("N"));

            var proxyTypeBuilder = new ProxyTypeBuilder();

            var generatedType = proxyTypeBuilder.GenerateType(parameters);

            return new GenerateProxyResult<ITestInterface>(
                generatedType.GeneratedType,
                generatedType.InterfacesImplemented,
                generatedType.GetFactory<ITestInterface, ITestInterface>());
        }

        private static GenerateTypeResult CreateProxyTypeWith2Interfaces()
        {
            var parameters = TypeCloneBuilderParameters<ProxyTypeBuilder.TypeContext, ProxyTypeBuilder.MethodContext>
                .New
                .AddInterface(typeof(IInterface1)).AddInterface(typeof(IInterface2))
                .ParentType(typeof(ProxyBase<,>).MakeGenericType(typeof(IInterface1), typeof(IInterface2)))
                .TypeName("MultiProxy" + "_" + Guid.NewGuid().ToString("N"));

            var proxyTypeBuilder = new ProxyTypeBuilder();

            return proxyTypeBuilder.GenerateType(parameters);
        }

        private ProxyAndTestImplementation<ITestInterface, TestInterfaceImplementation> GetProxyAndTest()
        {
            lock (proxyLock)
            {
                if (proxy.Factory == null)
                {
                    proxy = CreateProxyType(); 
                }
            }

            var test = new TestInterfaceImplementation();
            return new ProxyAndTestImplementation<ITestInterface, TestInterfaceImplementation>(proxy.Factory(test), test);
        }

        public struct GenericStruct<T>
        {
            public GenericStruct(T first, T second)
            {
                this.First = first;
                this.Second = second;
            }

            public T First { get; set; }

            public T Second { get; set; }
        }

        private struct ProxyAndTestImplementation<TInterface, TTestImplementation>
        {
            public ProxyAndTestImplementation(TInterface proxy, TTestImplementation test)
            {
                this.Proxy = proxy;
                this.Test = test;
            }

            public TInterface Proxy { get; }

            public TTestImplementation Test { get; }
        }
    }
}