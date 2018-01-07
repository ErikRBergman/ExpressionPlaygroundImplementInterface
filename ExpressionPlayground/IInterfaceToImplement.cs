namespace ExpressionPlayground
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IInterfaceToImplement
    {
        Task<Model> Result_Parameters_Async(int intParameter, string stringParameter);

        Task<int> Result_GenericParameter_Async<TModel>(TModel model);

        Task<TModel> GenericResult_GenericParameter_Async<TModel>(TModel model);

        Task<int> Result_Generic_NoParameters_Async<T1>();

        Task<int> Result_NoParameters_Async();
        
        Task<ThreeGenericParameter<T1, T2, T3>> GenericResult_GenericParameters_Async<T1, T2, T3>(T1 t1, T2 t2, T3 t3);

        Task NoResult_Parameters_Async(int intParameter, string stringParameter);

        Task NoResult_Generic_Parameters_Async<T1, T2, T3>(T1 t1, T2 t2, T3 t3);

        Task NoResult_NoParameters_Async();

        Task NoResult_Generic_NoParameters_Async<T1>();


        // Non async versions
        Model Result_Parameters(int intParameter, string stringParameter);

        int Result_GenericParameter<TModel>(TModel model);

        TModel GenericResult_GenericParameter<TModel>(TModel model);

        int Result_Generic_NoParameters<T1>();

        int Result_NoParameters();

        ThreeGenericParameter<T1, T2, T3> GenericResult_GenericParameters<T1, T2, T3>(T1 t1, T2 t2, T3 t3);

        void NoResult_Parameters(int intParameter, string stringParameter);

        void NoResult_Generic_Parameters<T1, T2, T3>(T1 t1, T2 t2, T3 t3);

        void NoResult_NoParameters();

        void NoResult_Generic_NoParameters<T1>();

        Task<T1> GenericsAndVarArgs<T1>(params T1[] parameters);

        void ComplexGenericStructure<T1, T2, T3>(KeyValuePair<KeyValuePair<T1, KeyValuePair<T2, T3>>, KeyValuePair<T2, KeyValuePair<T3, T2>>> parameter);
    }

    public struct ThreeGenericParameter<T1, T2, T3>
    {
        public T1 t1 { get; set; }

        public T2 t2 { get; set; }

        public T3 t3 { get; set; }

    }
}