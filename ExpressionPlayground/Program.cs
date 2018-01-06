namespace ExpressionPlayground
{
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
            var proxyTypeBuilder = new ProxyTypeBuilder(typeof(ProxyBase<>));
            var proxyTypeInformation = proxyTypeBuilder.GenerateProxy<IInterfaceToImplement>();

            DefaultValues.DefaultAssemblyBuilder.Save(DefaultValues.DefaultAssemblyBuilder.GetName().Name + ".dll");

            var testImplementation = new TestImplementation();

            var proxy = proxyTypeInformation.Factory(testImplementation);

            var validator = Validator.Default;

            var resultParameters = await proxy.Result_Parameters(123, "123");

            var methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.Result_Parameters), mc.MethodName) == 0);
            validator.AreEqual(2, methodCall.Parameters.Length);
            Validator.Default.AreEqual(resultParameters.IntValue, 123);

            var genericParameter = await proxy.Result_GenericParameter("B");
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.Result_GenericParameter), mc.MethodName) == 0);

            Validator.Default.AreEqual(genericParameter, 123);

            var resultGenericParameter = await proxy.GenericResult_GenericParameter("C");
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.GenericResult_GenericParameter), mc.MethodName) == 0);

            testImplementation.ClearMethodCalls();

            var genericResultGenericParameter = await proxy.GenericResult_GenericParameter(new KeyValuePair<string, int>("One", 1));
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.GenericResult_GenericParameter), mc.MethodName) == 0);

            var genericResultGenericParameters = await proxy.GenericResult_GenericParameters(1, "Dos", new KeyValuePair<int, string>(3, "Drei"));
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.GenericResult_GenericParameters), mc.MethodName) == 0);

            await proxy.NoResult_Parameters(5, "Five");
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.NoResult_Parameters), mc.MethodName) == 0);

            await proxy.NoResult_NoParameters();
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.NoResult_NoParameters), mc.MethodName) == 0);

            var result_noparameters = await proxy.Result_NoParameters();
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.Result_NoParameters), mc.MethodName) == 0);

            await proxy.NoResult_Generic_Parameters(1, 2, "haj");
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.NoResult_Generic_Parameters), mc.MethodName) == 0);

            await proxy.NoResult_Generic_NoParameters<string>();
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.NoResult_Generic_NoParameters), mc.MethodName) == 0);

            var result = proxy.Result_Generic_NoParameters<string>();
            methodCall = testImplementation.TestMethodCalls.Single(mc => string.CompareOrdinal(nameof(proxy.Result_Generic_NoParameters), mc.MethodName) == 0);
            
            // Todo: Methods returning void or non Task argument, including generic arguments
            // Todo: Methods with variable arguments
            // Todo: Generic interface
        }

    }
}