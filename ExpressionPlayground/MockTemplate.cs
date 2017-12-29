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
            var localModel = new SetModelClass<TModel>();
            localModel.Model = model;

            return this.inner.SetModel(localModel.Model);
        }

        public MockTemplate(IInterfaceToImplement inner)
            : base(inner)
        {
        }
    }

    public class SetModelClass<T>
    {
        public T Model { get; set; }
    }
}