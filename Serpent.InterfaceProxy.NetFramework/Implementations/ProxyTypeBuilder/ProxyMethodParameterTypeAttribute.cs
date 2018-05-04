namespace Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class ProxyMethodParameterTypeAttribute : Attribute
    {
        public ProxyMethodParameterTypeAttribute(ProxyMethodParameterType parameterType)
        {
            this.ParameterType = parameterType;
        }

        public ProxyMethodParameterType ParameterType { get; }
    }
}