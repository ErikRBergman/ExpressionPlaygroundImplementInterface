namespace Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter)]
    public class ProxyMethodParameterTypeAttribute : Attribute
    {
        public ProxyMethodParameterTypeAttribute(ProxyMethodParameterType parameterType)
        {
            this.ParameterType = parameterType;
        }

        public ProxyMethodParameterType ParameterType { get; }
    }
}