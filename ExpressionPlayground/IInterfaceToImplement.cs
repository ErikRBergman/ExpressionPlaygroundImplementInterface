namespace ExpressionPlayground
{
    using System.Threading.Tasks;

    public interface IInterfaceToImplement
    {
        Task<Model> Result_Parameters(int intParameter, string stringParameter);

        Task<int> Result_GenericParameter<TModel>(TModel model);

        Task<TModel> GenericResult_GenericParameter<TModel>(TModel model);

        Task<int> Result_Generic_NoParameters<T1>();

        Task<ThreeGenericParameter<T1, T2, T3>> GenericResult_GenericParameters<T1, T2, T3>(T1 t1, T2 t2, T3 t3);

        Task NoResult_Parameters(int intParameter, string stringParameter);

        Task NoResult_Generic_Parameters<T1, T2, T3>(T1 t1, T2 t2, T3 t3);

        Task NoResult_NoParameters();

        Task NoResult_Generic_NoParameters<T1>();

        //Task GenericsAndVarArgs<T1>(params T1[] parameters);
    }

    public struct ThreeGenericParameter<T1, T2, T3>
    {
        public T1 t1 { get; set; }

        public T2 t2 { get; set; }

        public T3 t3 { get; set; }

    }
}