namespace Serpent.IntermediateLanguageTools
{
    using System;
    using System.Reflection;

    using Serpent.IntermediateLanguageTools.Models;

    public class CreateMethodILGeneratorParameters
    {
        public CreateMethodILGeneratorParameters(MethodBase method, Type sourceType = null)
        {
            this.Method = method;
            this.SourceType = sourceType;
        }

        public MethodBase Method { get; }

        public Type SourceType { get; }

        public DotNetType DotNetType { get; set; }

        public int TabCount { get; set; }

        public bool Verbose { get; set; }
    }
}