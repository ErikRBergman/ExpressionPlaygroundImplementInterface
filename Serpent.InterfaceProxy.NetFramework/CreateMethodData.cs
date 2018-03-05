namespace Serpent.InterfaceProxy
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    public struct CreateMethodData
    {
        public Type SourceType { get; set; }

        public TypeBuilder TypeBuilder { get; set; }

        public MethodInfo SourceMethodInfo { get; set; }

        public IEnumerable<TypeBuilderMethodParameter> Parameters { get; set; }

        public Type[] GenericArguments { get; set; }

        public string[] GenericArgumentNames { get; set; }
    }
}