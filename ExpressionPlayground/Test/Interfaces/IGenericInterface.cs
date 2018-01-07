namespace ExpressionPlayground.Test.Interfaces
{
    using System.Collections.Generic;

    public interface IGenericInterface<T1, T2, T3>
    {
        KeyValuePair<T1, T2> ClassAndMethodGenericArguments<MT1, MT2>(KeyValuePair<T1, KeyValuePair<T2, KeyValuePair<MT1, MT2>>> parameter);

        KeyValuePair<T1, T2> ClassGenericArguments(KeyValuePair<T1, T2> parameter);
    }
}