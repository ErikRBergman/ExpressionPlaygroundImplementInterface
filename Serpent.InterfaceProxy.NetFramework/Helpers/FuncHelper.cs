namespace Serpent.InterfaceProxy.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Serpent.InterfaceProxy.Extensions;

    internal static class FuncHelper
    {
        public static Type CreateRaw(params Type[] parameters)
        {
            return Get(parameters.Length).MakeGenericType(parameters);
        }

        public static Type Create(Type returnValue, params Type[] parameters)
        {
            return Get(parameters.Length + 1).MakeGenericType(parameters.Prepend(returnValue).ToArray());
        }

        public static Type Create(Type returnValue, IEnumerable<Type> parameters)
        {
            var types = parameters.Append(returnValue).ToArray();

            var funcType = Get(types.Length);

            return funcType.MakeGenericType(types);
        }


        public static Type Get(int parameterCount)
        {
            switch (parameterCount)
            {
                case 1: return typeof(Func<>);
                case 2: return typeof(Func<,>);
                case 3: return typeof(Func<,,>);
                case 4: return typeof(Func<,,,>);
                case 5: return typeof(Func<,,,,>);
                case 6: return typeof(Func<,,,,,>);
                case 7: return typeof(Func<,,,,,,>);
                case 8: return typeof(Func<,,,,,,,>);
                case 9: return typeof(Func<,,,,,,,,>);
                case 10: return typeof(Func<,,,,,,,,,>);
                case 11: return typeof(Func<,,,,,,,,,,>);
                case 12: return typeof(Func<,,,,,,,,,,,>);
                case 13: return typeof(Func<,,,,,,,,,,,,>);
                case 14: return typeof(Func<,,,,,,,,,,,,,>);
                case 15: return typeof(Func<,,,,,,,,,,,,,,>);
                case 16: return typeof(Func<,,,,,,,,,,,,,,,>);
                case 17: return typeof(Func<,,,,,,,,,,,,,,,,>);

                default:
                    throw new Exception("Func supports at most 16 parameters, son");
            }
        }
    }
}