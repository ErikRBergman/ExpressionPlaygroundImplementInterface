namespace ExpressionPlayground
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;

    using ExpressionPlayground.Extensions;
    using ExpressionPlayground.Test;
    using ExpressionPlayground.Test.Interfaces;
    using ExpressionPlayground.Types;
    using ExpressionPlayground.Validation;

    using Serpent.Common.BaseTypeExtensions.Collections;

    public class Program
    {
        private static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            var newType = SubstituteTypes<decimal, double, byte, string, float, short>(new KeyValuePair<KeyValuePair<decimal, KeyValuePair<double, byte>>, KeyValuePair<double, KeyValuePair<byte, double>>>());

            var proxyTypeBuilder = new ProxyTypeBuilder(typeof(ProxyBase<>))
            {
                ClosureTypeNameSelector = (@interface, methodInfo, @namespace) =>
                    @namespace + "." + @interface.Name + "." + methodInfo.Name + "_Closure"
            };

            var proxyTypeInformation = proxyTypeBuilder.GenerateProxy<ITestInterface>();

            DefaultValues.DefaultAssemblyBuilder.Save(DefaultValues.DefaultAssemblyBuilder.GetName().Name + ".dll");

            var testImplementation = new TestInterfaceImplementation();

            var proxy = proxyTypeInformation.Factory(testImplementation);

            var validator = Validator.Default;

            var resultParameters = await proxy.Result_Parameters_Async(123, "123");

            var methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.Result_Parameters_Async), mc.MethodName) == 0);
            validator.AreEqual(2, methodCall.Parameters.Length);
            Validator.Default.AreEqual(resultParameters.IntValue, 123);

            var genericParameter = await proxy.Result_GenericParameter_Async("B");
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.Result_GenericParameter_Async), mc.MethodName) == 0);

            Validator.Default.AreEqual(genericParameter, 123);

            var resultGenericParameter = await proxy.GenericResult_GenericParameter_Async("C");
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.GenericResult_GenericParameter_Async), mc.MethodName) == 0);

            testImplementation.ClearMethodCalls();

            var genericResultGenericParameter = await proxy.GenericResult_GenericParameter_Async(new KeyValuePair<string, int>("One", 1));
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.GenericResult_GenericParameter_Async), mc.MethodName) == 0);

            var genericResultGenericParameters = await proxy.GenericResult_GenericParameters_Async(1, "Dos", new KeyValuePair<int, string>(3, "Drei"));
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.GenericResult_GenericParameters_Async), mc.MethodName) == 0);

            await proxy.NoResult_Parameters_Async(5, "Five");
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.NoResult_Parameters_Async), mc.MethodName) == 0);

            await proxy.NoResult_NoParameters_Async();
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.NoResult_NoParameters_Async), mc.MethodName) == 0);

            var result_noparameters = await proxy.Result_NoParameters_Async();
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.Result_NoParameters_Async), mc.MethodName) == 0);

            await proxy.NoResult_Generic_Parameters_Async(1, 2, "haj");
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.NoResult_Generic_Parameters_Async), mc.MethodName) == 0);

            await proxy.NoResult_Generic_NoParameters_Async<string>();
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.NoResult_Generic_NoParameters_Async), mc.MethodName) == 0);

            var result = proxy.Result_Generic_NoParameters_Async<string>();
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.Result_Generic_NoParameters_Async), mc.MethodName) == 0);

            ////Model Result_Parameters(int intParameter, string stringParameter);
            var result_Parameters = proxy.Result_Parameters(1, "uno");
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.Result_Parameters), mc.MethodName) == 0);

            ////int Result_GenericParameter<TModel>(TModel model);
            var result_GenericParameter = proxy.Result_GenericParameter("uno");
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.Result_GenericParameter), mc.MethodName) == 0);

            ////TModel GenericResult_GenericParameter<TModel>(TModel model);
            var genericResult_GenericParameter = proxy.GenericResult_GenericParameter("uno");
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.GenericResult_GenericParameter), mc.MethodName) == 0);

            ////int Result_Generic_NoParameters<T1>();
            var result_Generic_NoParameters = proxy.Result_Generic_NoParameters<string>();
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.Result_Generic_NoParameters), mc.MethodName) == 0);

            ////int Result_NoParameters();
            var result_NoParameters = proxy.Result_NoParameters();
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.Result_NoParameters), mc.MethodName) == 0);

            ////ThreeGenericParameter<T1, T2, T3> GenericResult_GenericParameters<T1, T2, T3>(T1 t1, T2 t2, T3 t3);
            var genericResult_GenericParameters = proxy.GenericResult_GenericParameters(1, 0.2m, new ThreeGenericParameter<string, int, float>());
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.GenericResult_GenericParameters), mc.MethodName) == 0);

            ////void NoResult_Parameters(int intParameter, string stringParameter);
            proxy.NoResult_Parameters(6, "Six");
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.NoResult_Parameters), mc.MethodName) == 0);

            ////void NoResult_Generic_Parameters<T1, T2, T3>(T1 t1, T2 t2, T3 t3);
            proxy.NoResult_Generic_Parameters(1, 0.2m, new ThreeGenericParameter<string, int, float>());
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.NoResult_Generic_Parameters), mc.MethodName) == 0);

            ////void NoResult_NoParameters();
            proxy.NoResult_NoParameters();
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.NoResult_NoParameters), mc.MethodName) == 0);

            ////void NoResult_Generic_NoParameters<T1>();
            proxy.NoResult_Generic_NoParameters<float>();
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.NoResult_Generic_NoParameters), mc.MethodName) == 0);

            ////void GenericsAndVarArgs<T1>();
            var genericsAndVarArgsResult = proxy.GenericsAndVarArgs(1, 2 ,3 ,4);

            // var genericsAndVarArgsResult = proxy.GenericsAndVarArgs(new[] { 1, 2, 3, 4 });
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.GenericsAndVarArgs), mc.MethodName) == 0);

            var complexGenericStructureObject =
                new KeyValuePair<KeyValuePair<int, KeyValuePair<string, GenericStruct<int>>>, KeyValuePair<string, KeyValuePair<GenericStruct<int>, string>>>(
                    new KeyValuePair<int, KeyValuePair<string, GenericStruct<int>>>(
                        55, 
                        new KeyValuePair<string, GenericStruct<int>>("fivefive", new GenericStruct<int>(1, 2))),
                    new KeyValuePair<string, KeyValuePair<GenericStruct<int>, string>>("two", new KeyValuePair<GenericStruct<int>, string>(new GenericStruct<int>(5, 6), "fivesix")));

            var complexGenericStructure = proxy.ComplexGenericStructure(complexGenericStructureObject);
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.ComplexGenericStructure), mc.MethodName) == 0);

            if (complexGenericStructure.Key.Key != 55)
            {
                throw new Exception("Bummer!");
            }

            // Todo: Generic interfaces
            // Todo: Change the closure classes into structs to prevent the extra heap allocation
            // Todo: Add support for properties

            // Usage for the proxy:
            // * Service fabric auto repartitioning (where the idea came up)
            // * Creating decorators like the Serpent.Chain decorators - (cache, retry, aggregate, logging, performance measuring, semaphores, concurrency)
            // 
            // Usage for implementing an interface dynamically
            // * Interface to Web API in ASP.NET Core
            //   The service developer creates an interface and a service. The middleware implements the interface as a service, or perhaps a type inheriting from Controller and have MVC handle routing (and other services like Open API will work out of the box)
        }

        private static Type SubstituteTypes<T1, T2, T3, S1, S2, S3>(params KeyValuePair<KeyValuePair<T1, KeyValuePair<T2, T3>>, KeyValuePair<T2, KeyValuePair<T3, T2>>>[] parameters)
        {
            var substitutes = ImmutableDictionary<Type, Type>.Empty.Add(typeof(T1), typeof(S1)).Add(typeof(T2), typeof(S2)).Add(typeof(T3), typeof(S3));
            var type = parameters.GetType();
            return TypeSubstitutor.GetSubstitutedType(type, substitutes);
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
    }
}