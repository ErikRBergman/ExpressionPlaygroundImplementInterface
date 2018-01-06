namespace ExpressionPlayground.Extensions
{
    using System;
    using System.Reflection;

    internal static class MethodInfoExtensions
    {
        public static MethodInfo MakeGenericMethodIfNecessary<TMI>(this TMI methodInfo, params Type[] parameters)
            where TMI : MethodInfo
        {
            if (parameters.Length > 0)
            {
                return methodInfo.MakeGenericMethod(parameters);
            }

            return methodInfo;
        }
    }
}