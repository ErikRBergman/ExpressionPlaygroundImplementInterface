using System;
using System.Collections.Generic;

using Serpent.Common.BaseTypeExtensions.Collections;

static internal class TypeSubstitutor
{
    public static Type SubstituteTypes(Type mainType, IDictionary<Type, Type> substitutes)
    {
        var itemType = mainType;

        int arrayRank = 0;

        if (mainType.IsArray)
        {
            itemType = mainType.GetElementType();
            arrayRank = mainType.GetArrayRank();
        }

        itemType = substitutes.GetValueOrDefault(itemType, itemType);

        Type newType = itemType;

        var genericArguments = itemType.GetGenericArguments();

        if (genericArguments.Length != 0)
        {
            var newGenericArguments = new List<Type>(genericArguments.Length);

            foreach (var genericArgument in genericArguments)
            {
                var newGenericArgument = substitutes.GetValueOrDefault(genericArgument, genericArgument);

                if (newGenericArgument == genericArgument)
                {
                    newGenericArgument = SubstituteTypes(newGenericArgument, substitutes);
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