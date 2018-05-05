namespace Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder
{
    using Serpent.InterfaceProxy.Core;

    public class ProxyBuilder : TypeCloneBuilderParameters
    {
        public static new ProxyBuilder New => new ProxyBuilder();

        public GenerateTypeResult Build()
        {
            var proxyTypeBuilder = new ProxyTypeBuilder();
            return proxyTypeBuilder.GenerateType(this);
        }
    }
}