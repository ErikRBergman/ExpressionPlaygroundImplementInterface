namespace ExpressionPlayground
{
    using System;
    using System.Collections.Generic;

    public static class DynamicImplementInterfaceResultExtensions
    {
        public static DynamicImplementInterfaceResult Add(this DynamicImplementInterfaceResult dynamicImplementInterfaceResult, DynamicImplementInterfaceResult other)
        {
            return new DynamicImplementInterfaceResult(dynamicImplementInterfaceResult.InterfacesImplemented.AddRange(other.InterfacesImplemented), dynamicImplementInterfaceResult.NamesUsed.AddRange(other.NamesUsed));
        }

        public static DynamicImplementInterfaceResult AddImplementedInterface(this DynamicImplementInterfaceResult dynamicImplementInterfaceResult, Type interfaceType)
        {
            return new DynamicImplementInterfaceResult(dynamicImplementInterfaceResult.InterfacesImplemented.Add(interfaceType), dynamicImplementInterfaceResult.NamesUsed);
        }

        public static DynamicImplementInterfaceResult AddUsedNames(this DynamicImplementInterfaceResult dynamicImplementInterfaceResult, IEnumerable<string> usedNames)
        {
            return new DynamicImplementInterfaceResult(dynamicImplementInterfaceResult.InterfacesImplemented, dynamicImplementInterfaceResult.NamesUsed.AddRange(usedNames));
        }
    }
}