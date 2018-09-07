namespace Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder
{
    using System;

    /// <summary>
    /// Marks a method to be used as proxy interception method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ProxyMethodAttribute : Attribute
    {
    }
}