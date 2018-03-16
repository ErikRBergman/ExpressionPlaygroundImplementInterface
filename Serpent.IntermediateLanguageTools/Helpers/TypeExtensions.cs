namespace Serpent.IntermediateLanguageTools.Helpers
{
    using System;
    using System.Linq;

    public static class TypeExtensions
    {
        public static string GetCSharpName(this Type type, bool includeNamespace = false, bool includeGlobal = false)
        {
            var name = includeNamespace ? type.FullName : type.Name;

            if (name == null)
            {
                return null;
            }

            var separatorIndex = name.IndexOf('`');

            if (separatorIndex != -1)
            {
                name = name.Substring(0, separatorIndex);
            }

            if (includeGlobal)
            {
                name = "global::" + name;
            }

            var genericParameters = type.GetGenericArguments();

            // generic type
            if (genericParameters.Any(gp => gp.IsGenericParameter))
            {
                    // example: KeyValuePair<,>
                    return name + $"<{string.Join(",", Enumerable.Repeat(string.Empty, genericParameters.Length))}>";
            }
            else
            {
                // generic implementation
                if (genericParameters.Length > 0)
                {
                    // example: KeyValuePair<int, string> or KeyValuePair<int, KeyValuePair<bool, string>>
                    return name + $"<{string.Join(", ", genericParameters.Select(gp => gp.GetCSharpName(includeNamespace)))}>";
                }
            }

            return name;
        }
    }
}