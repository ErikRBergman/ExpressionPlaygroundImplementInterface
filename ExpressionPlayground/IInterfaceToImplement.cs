namespace ExpressionPlayground
{
    using System.Threading.Tasks;

    public interface IInterfaceToImplement
    {
        Task<Model> A(int intParameter, string stringParameter);

        Task<int> B<TModel>(TModel model);


        Task<TModel> C<TModel>(TModel model);

        Task<ThreeGenericParameter<T1, T2, T3>> D<T1, T2, T3>(T1 t1, T2 t2, T3 t3);

        Task E(int intParameter, string stringParameter);

        //Task F<T1, T2, T3>(T1 t1, T2 t2, T3 t3);

        //Task G<T1>();

        //Task H<T1>(params T1[] parameters);
    }

    public struct ThreeGenericParameter<T1, T2, T3>
    {
        public T1 t1 { get; set; }

        public T2 t2 { get; set; }

        public T3 t3 { get; set; }

    }
}