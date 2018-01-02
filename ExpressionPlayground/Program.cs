namespace ExpressionPlayground
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ExpressionPlayground.Extensions;
    using ExpressionPlayground.Validation;

    public class Program
    {
        private static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            var g = new ProxyTypeBuilder();
            var proxy = g.GenerateProxy<IInterfaceToImplement>();

            var testImplementation = new TestImplementation();

            var dynamicType = (IInterfaceToImplement)Activator.CreateInstance(proxy.GeneratedType, testImplementation);

            var validator = Validator.Default;

            var resultParameters = await dynamicType.Result_Parameters(123, "123");

            var methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(dynamicType.Result_Parameters), mc.MethodName) == 0);
            validator.AreEqual(2, methodCall.Parameters.Length);
            Validator.Default.AreEqual(resultParameters.IntValue, 123);

            var genericParameter = await dynamicType.Result_GenericParameter("B");
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(dynamicType.Result_GenericParameter), mc.MethodName) == 0);

            Validator.Default.AreEqual(genericParameter, 123);

            var resultGenericParameter = await dynamicType.GenericResult_GenericParameter("C");
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(dynamicType.GenericResult_GenericParameter), mc.MethodName) == 0);

            testImplementation.ClearMethodCalls();

            var genericResultGenericParameter = await dynamicType.GenericResult_GenericParameter(new KeyValuePair<string, int>("One", 1));
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(dynamicType.GenericResult_GenericParameter), mc.MethodName) == 0);

            var genericResultGenericParameters = await dynamicType.GenericResult_GenericParameters(1, "Dos", new KeyValuePair<int, string>(3, "Drei"));
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(dynamicType.GenericResult_GenericParameters), mc.MethodName) == 0);

            await dynamicType.NoResult_Parameters(5, "Five");
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(dynamicType.NoResult_Parameters), mc.MethodName) == 0);

            await dynamicType.NoResult_NoParameters();
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(dynamicType.NoResult_NoParameters), mc.MethodName) == 0);

            await dynamicType.NoResult_Generic_NoParameters<string>();
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(dynamicType.NoResult_Generic_NoParameters), mc.MethodName) == 0);

            var result = dynamicType.Result_Generic_NoParameters<string>();
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(dynamicType.Result_Generic_NoParameters), mc.MethodName) == 0);

            await dynamicType.NoResult_Generic_Parameters(1, 2, "haj");
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(dynamicType.NoResult_Generic_Parameters), mc.MethodName) == 0);
            
            DefaultValues.DefaultAssemblyBuilder.Save(DefaultValues.DefaultAssemblyBuilder.GetName().Name + ".dll");
        }

    }
}