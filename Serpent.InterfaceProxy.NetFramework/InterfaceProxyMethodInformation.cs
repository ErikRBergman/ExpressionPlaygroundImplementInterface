namespace Serpent.InterfaceProxy
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Serpent.InterfaceProxy.Core;

    public struct InterfaceProxyMethodInformation
    {
        public string[] GenericArgumentNames { get; set; }

        public Type[] GenericArguments { get; set; }

        public IEnumerable<TypeBuilderMethodParameter> Parameters { get; set; }

        public MethodInfo SourceMethodInfo { get; set; }

        public Type SourceType { get; set; }

        public TypeBuilder TypeBuilder { get; set; }
    }
}