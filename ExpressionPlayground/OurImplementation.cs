namespace ExpressionPlayground
{
    using System.Threading.Tasks;

    public class OurImplementation : IInterfaceToImplement
    {
        public Task<Model> A(int intParameter, string stringParameter)
        {
            return Task.FromResult(
                new Model
                {
                    IntValue = intParameter,
                    StringValue = stringParameter
                });
        }

        public Task<int> B<TModel>(TModel model)
        {
            return Task.FromResult(123);
        }

        public Task<TModel> C<TModel>(TModel model)
        {
            return Task.FromResult(model);
        }

        public Task<ThreeGenericParameter<T1, T2, T3>> D<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            return Task.FromResult(
                new ThreeGenericParameter<T1, T2, T3>
                {
                    t1 = t1,

                    t2 = t2,

                    t3 = t3
                });
        }

        public Task E(int intParameter, string stringParameter)
        {
            return Task.CompletedTask;
        }

        public Task F<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            return Task.CompletedTask;
        }

        public Task G<T1>()
        {
            var type = typeof(T1);
            return Task.CompletedTask;
        }
    }
}
