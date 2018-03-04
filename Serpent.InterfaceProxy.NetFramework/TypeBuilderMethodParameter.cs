namespace Serpent.InterfaceProxy
{
    using System;
    using System.Reflection;

    public class TypeBuilderMethodParameter
    {
        public TypeBuilderMethodParameter()
        {
        }

        public TypeBuilderMethodParameter(ParameterInfo sourceParameter)
        {
            this.SourceParameter = sourceParameter;
            this.DefaultValue = sourceParameter.DefaultValue;
            this.HasDefaultValue = sourceParameter.HasDefaultValue;
            this.HasParamArrayArgument = sourceParameter.GetCustomAttribute<ParamArrayAttribute>() != null;
            this.Name = sourceParameter.Name;
            this.Attributes = sourceParameter.Attributes;
            this.ParameterType = sourceParameter.ParameterType;
        }

        public object DefaultValue { get; set; }

        public bool HasDefaultValue { get; set; }

        public bool HasParamArrayArgument { get; set; }

        public string Name { get; set; }

        public ParameterAttributes Attributes { get; set; }

        public Type ParameterType { get; set; }

        public ParameterInfo SourceParameter { get; }
    }
}