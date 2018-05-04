using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder
{
    using System.Reflection;

    /// <summary>
    /// Provides type extensions for the Proxy Type Builder
    /// </summary>
    public static class MethodInfoExtensions
    {
        public static ProxyMethodParameter[] GetProxyMethodParameters(this MethodInfo methodInfo)
        {
            var parameters = new List<ProxyMethodParameter>();

            foreach (var parameter in methodInfo.GetParameters())
            {
                var attribute = parameter.GetCustomAttribute<ProxyMethodParameterTypeAttribute>(true);

                ProxyMethodParameterType parameterType = ProxyMethodParameterType.AutoDetect;

                if (attribute != null)
                {
                    parameterType = attribute.ParameterType;
                }

                parameters.Add(new ProxyMethodParameter(parameter, parameterType));
            }

            return parameters.ToArray();
        }
    }

    public class ProxyMethodParameter
    {
        public ParameterInfo ParameterInfo { get; }

        public ProxyMethodParameterType ProxyMethodParameterType { get; }

        public ProxyMethodParameter(ParameterInfo parameterInfo, ProxyMethodParameterType proxyMethodParameterType)
        {
            this.ParameterInfo = parameterInfo;
            this.ProxyMethodParameterType = proxyMethodParameterType;
        }

    }
}