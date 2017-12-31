namespace ExpressionPlayground
{
    using System.Threading.Tasks;

    public class OurImplementation : IInterfaceToImplement
    {
        public Task<Model> A(int intParameter, string stringParameter)
        {
            return Task.FromResult(
                new Model()
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
    }
}
