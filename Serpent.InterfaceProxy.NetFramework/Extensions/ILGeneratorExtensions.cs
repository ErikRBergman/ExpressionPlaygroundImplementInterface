// ReSharper disable StyleCop.SA1400
namespace Serpent.InterfaceProxy.Extensions
{
    using System;
    using System.Reflection.Emit;

    public static class ILGeneratorExtensions
    {
        public static ILGenerator LdArg(this ILGenerator generator, int argumentNumber)
        {
            switch (argumentNumber)
            {
                case 0:
                    generator.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Ldarg_3);
                    break;
                default:

                    if (argumentNumber <= byte.MaxValue)
                    {
                        generator.Emit(OpCodes.Ldarg_S, Convert.ToByte(argumentNumber));
                    }
                    else
                    {
                        generator.Emit(OpCodes.Ldarg, argumentNumber);
                    }

                    break;
            }

            return generator;
        }
    }
}