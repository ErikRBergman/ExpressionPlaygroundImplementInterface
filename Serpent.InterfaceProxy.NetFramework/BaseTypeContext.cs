namespace Serpent.InterfaceProxy
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    public class BaseTypeContext
    {
        public IReadOnlyCollection<Type> InterfacesToImplement { get; set; }

        public TypeBuilder TypeBuilder { get; set; }

        public TypeCloneBuilderParameters Parameters { get; set; }

    }
}