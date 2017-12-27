namespace ExpressionPlayground
{
    using System.Threading.Tasks;

    public class MockTemplate : ProxyBase<IInterfaceToImplement>, IInterfaceToImplement
    {
        public Task<Model> GetModel(int intParameter, string stringParameter)
        {
            return this.ExecuteAsync(inner => inner.GetModel(intParameter, stringParameter));
            // return this.inner.GetModel(intParameter, stringParameter);
        }

        public Task<TModel> SetModel<TModel>(TModel model)
        {
            return this.inner.SetModel(model);
        }

        public MockTemplate(IInterfaceToImplement inner)
            : base(inner)
        {
        }
    }
}