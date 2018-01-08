// ReSharper disable UnusedVariable
// ReSharper disable RedundantAssignment
namespace ExpressionPlayground
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Serpent.InterfaceProxy;
    using Serpent.InterfaceProxy.Extensions;
    using Serpent.InterfaceProxy.NetFramework.Tests.Test;
    using Serpent.InterfaceProxy.NetFramework.Tests.Test.Interfaces;
    using Serpent.InterfaceProxy.Types;

    public class Program
    {
        private static void Main()
        {
            MainAsync().Wait();
        }

        private static async Task MainAsync()
        {
            var newType = SubstituteTypes<decimal, double, byte, string, float, short>(
                new KeyValuePair<KeyValuePair<decimal, KeyValuePair<double, byte>>, KeyValuePair<double, KeyValuePair<byte, double>>>());

            var proxyTypeBuilder = new ProxyTypeBuilder(typeof(ProxyBase<>))
                                       {
                                           ClosureTypeNameSelector = (@interface, methodInfo, @namespace) =>
                                               @namespace + "." + @interface.Name + "." + methodInfo.Name + "_Closure"
                                       };

            var proxyTypeInformation = proxyTypeBuilder.GenerateProxy<ITestInterface>();

            DefaultValues.DefaultAssemblyBuilder.Save(DefaultValues.DefaultAssemblyBuilder.GetName().Name + ".dll");

            await Task.Yield();
           
            // Todo: Generic interfaces
            // Todo: Return the proxy method chosen
            // Todo: Change the closure classes into structs to prevent the extra heap allocation
            // Todo: Add support for properties

            // Usage for the proxy:
            // * Service fabric auto repartitioning (where the idea came up)
            // * Creating decorators like the Serpent.Chain decorators - (cache, retry, aggregate, logging, performance measuring, semaphores, concurrency)
            // Usage for implementing an interface dynamically
            // * Interface to Web API in ASP.NET Core
            // The service developer creates an interface and a service. The middleware implements the interface as a service, or perhaps a type inheriting from Controller and have MVC handle routing (and other services like Open API will work out of the box)
        }

        private static Type SubstituteTypes<T1, T2, T3, S1, S2, S3>(
            params KeyValuePair<KeyValuePair<T1, KeyValuePair<T2, T3>>, KeyValuePair<T2, KeyValuePair<T3, T2>>>[] parameters)
        {
            var substitutes = new Dictionary<Type, Type>().Addf(typeof(T1), typeof(S1)).Addf(typeof(T2), typeof(S2)).Addf(typeof(T3), typeof(S3));
            var type = parameters.GetType();
            return TypeSubstitutor.GetSubstitutedType(type, substitutes);
        }
    }
}