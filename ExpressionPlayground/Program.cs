namespace ExpressionPlayground
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class Program
    {
        private static void Main(string[] args)
        {

#if DEBUG
            var assemblyName = "Serpent.ProxyBuilder.Debug";
#else
            string assemblyUniqueName = Guid.NewGuid().ToString("N");
            var assemblyName = "serpent.ProxyBuilder_" + assemblyUniqueName;
#endif

            var g = new ProxyTypeGenerator<IInterfaceToImplement>(assemblyName);
            var proxyType = g.GenerateProxy();

            var dynamicType = (IInterfaceToImplement)Activator.CreateInstance(proxyType.GeneratedType, new OurImplementation());

            var model = dynamicType.A(123, "123");

            model = dynamicType.A(123, "123");
            model = dynamicType.A(123, "123");
            model = dynamicType.A(123, "123");

            var setmodel1 = dynamicType.B("B");

            var setmodelC1 = dynamicType.C("C");

            var setmodelC2 = dynamicType.C(new KeyValuePair<string, int>("One", 1));

            var modelD = dynamicType.D(1, "Dos", new KeyValuePair<int, string>(3, "Drei"));

        }

    }
}