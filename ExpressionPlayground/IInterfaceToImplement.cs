namespace ExpressionPlayground
{
    using System.Threading.Tasks;

    public interface IInterfaceToImplement
    {
        Task<Model> A(int intParameter, string stringParameter);

        Task<int> B<TModel>(TModel model);


        Task<TModel> C<TModel>(TModel model);

        Task<ThreeGenericParameter<T1, T2, T3>> D<T1, T2, T3>(T1 t1, T2 t2, T3 t3);
    }

    public struct ThreeGenericParameter<T1, T2, T3>
    {
        public T1 t1 { get; set; }

        public T2 t2 { get; set; }

        public T3 t3 { get; set; }

    }
}