namespace Serpent.InterfaceProxy.Types
{
    using System;
    using System.Collections.Generic;

    using Serpent.InterfaceProxy.Extensions;

    public static class TypeSubstitutor
    {
        public static Type GetSubstitute(this IReadOnlyDictionary<Type, Type> substitutes, Type mainType)
        {
            return GetSubstitutedType(mainType, substitutes);
        }

        public static Type GetSubstitutedType(Type mainType, IReadOnlyDictionary<Type, Type> substitutes)
        {
            var itemType = mainType;

            var arrayRank = 0;

            // This method can be optimized by extracting the the parts that does not check and produce array and using it inside the recursion
            if (mainType.IsArray)
            {
                itemType = mainType.GetElementType();
                arrayRank = mainType.GetArrayRank();
            }

            itemType = substitutes.GetValueOrDefault(itemType, itemType);

            var newType = itemType;

            var genericArguments = itemType.GenericTypeArguments;

            if (genericArguments.Length != 0)
            {
                var newGenericArguments = new List<Type>(genericArguments.Length);

                foreach (var genericArgument in genericArguments)
                {
                    var newGenericArgument = substitutes.GetValueOrDefault(genericArgument, genericArgument);

                    // Make recursion if the type was not substituted
                    if (newGenericArgument == genericArgument)
                    {
                        newGenericArgument = GetSubstitutedType(newGenericArgument, substitutes);
                    }

                    newGenericArguments.Add(newGenericArgument);
                }

                newType = itemType.GetGenericTypeDefinition().MakeGenericType(newGenericArguments.ToArray());
            }

            if (mainType.IsArray)
            {
                if (arrayRank != 1)
                {
                    return newType.MakeArrayType(arrayRank);
                }

                return newType.MakeArrayType();
            }

            return newType;
        }
    }
}