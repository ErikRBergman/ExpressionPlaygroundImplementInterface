namespace ExpressionPlayground
{
    using System;
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

            var g = new ProxyGenerator<IInterfaceToImplement>(assemblyName);
            var proxyType = g.GenerateProxy();

            var dynamicType = (IInterfaceToImplement)Activator.CreateInstance(proxyType, new OurImplementation());

            var model = dynamicType.A(123, "123");

            model = dynamicType.A(123, "123");
            model = dynamicType.A(123, "123");
            model = dynamicType.A(123, "123");

            var setmodel1 = dynamicType.B("B");

            var setmodel2 = dynamicType.C("C");
        }

    }
}