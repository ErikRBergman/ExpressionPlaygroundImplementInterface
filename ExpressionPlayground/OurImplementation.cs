namespace ExpressionPlayground
{
    using System.Threading.Tasks;

    public class OurImplementation : IInterfaceToImplement
    {
        public Task<Model> GetModel(int intParameter, string stringParameter)
        {
            return Task.FromResult(
                new Model()
                {
                    IntValue = intParameter,
                    StringValue = stringParameter
                });
        }

        public Task<TModel> SetModel<TModel>(TModel model)
        {
            return Task.FromResult(model);
        }
    }
}
