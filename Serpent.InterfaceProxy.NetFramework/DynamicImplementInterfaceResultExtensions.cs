namespace Serpent.InterfaceProxy
{
    using System;
    using System.Collections.Generic;

    public static class DynamicImplementInterfaceResultExtensions
    {
        public static ImplementInterfaceMethodResult Add(this ImplementInterfaceMethodResult implementInterfaceMethodResult, ImplementInterfaceMethodResult other)
        {
            return new ImplementInterfaceMethodResult(implementInterfaceMethodResult.InterfacesImplemented.AddRange(other.InterfacesImplemented), implementInterfaceMethodResult.NamesUsed.AddRange(other.NamesUsed));
        }

        public static ImplementInterfaceMethodResult AddImplementedInterface(this ImplementInterfaceMethodResult implementInterfaceMethodResult, Type interfaceType)
        {
            return new ImplementInterfaceMethodResult(implementInterfaceMethodResult.InterfacesImplemented.Add(interfaceType), implementInterfaceMethodResult.NamesUsed);
        }

        public static ImplementInterfaceMethodResult AddUsedNames(this ImplementInterfaceMethodResult implementInterfaceMethodResult, IEnumerable<string> usedNames)
        {
            return new ImplementInterfaceMethodResult(implementInterfaceMethodResult.InterfacesImplemented, implementInterfaceMethodResult.NamesUsed.AddRange(usedNames));
        }
    }
}