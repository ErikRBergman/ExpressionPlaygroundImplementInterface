﻿namespace Serpent.InterfaceProxy
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    public class BaseTypeContext<TTypeContext, TMethodContext>
        where TTypeContext : BaseTypeContext<TTypeContext, TMethodContext>
        where TMethodContext : BaseMethodContext
    {
        public IReadOnlyCollection<Type> InterfacesToImplement { get; set; }

        public TypeBuilder TypeBuilder { get; set; }

        public Type SourceType { get; set; }

        public TypeCloneBuilderParameters<TTypeContext, TMethodContext> Parameters { get; set; }
    }
}