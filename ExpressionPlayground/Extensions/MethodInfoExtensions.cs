namespace ExpressionPlayground.Extensions
{
    using System;
    using System.Reflection;

    internal static class MethodInfoExtensions
    {
        public static MethodInfo MakeGenericMethodIfNecessary(this MethodInfo methodInfo, params Type[] parameters)
        {
            if (parameters.Length > 0)
            {
                return methodInfo.MakeGenericMethod(parameters);
            }

            return methodInfo;
        }
    }
}