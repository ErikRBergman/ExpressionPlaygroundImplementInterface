namespace ExpressionPlayground
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using ExpressionPlayground.Extensions;

    public class Program
    {
        private static void Main(string[] args)
        {

            var g = new ProxyTypeBuilder();
            var proxy = g.GenerateProxy<IInterfaceToImplement>();

            var dynamicType = (IInterfaceToImplement)Activator.CreateInstance(proxy.GeneratedType, new OurImplementation());

            var model = dynamicType.A(123, "123");

            model = dynamicType.A(123, "123");
            model = dynamicType.A(123, "123");
            model = dynamicType.A(123, "123");

            var setmodel1 = dynamicType.B("B");

            var setmodelC1 = dynamicType.C("C");

            var setmodelC2 = dynamicType.C(new KeyValuePair<string, int>("One", 1));

            var modelD = dynamicType.D(1, "Dos", new KeyValuePair<int, string>(3, "Drei"));

            var modelE = dynamicType.E(5, "Five");

            DefaultValues.DefaultAssemblyBuilder.Save(DefaultValues.DefaultAssemblyBuilder.GetName().Name + ".dll");
        }

    }
}