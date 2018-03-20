namespace Serpent.IntermediateLanguageTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class DotNetType
    {
        public DotNetType(Type sourceType)
        {
            this.SourceType = sourceType;
        }

        public Type SourceType { get; }

        public HashSet<string> FieldNames { get; } = new HashSet<string>();
    }
}