namespace ExpressionPlayground
{
    using System.Threading.Tasks;

    public class MockTemplate : ProxyBase<IInterfaceToImplement>, IInterfaceToImplement
    {
        public MockTemplate(IInterfaceToImplement inner)
            : base(inner)
        {
        }

        public Task<Model> GetModel(int intParameter, string stringParameter)
        {
            var getModel = new GetModelClass
                               {
                                   intParameter = intParameter,
                                   stringParameter = stringParameter
                               };

            return this.ExecuteAsync(getModel, this.GetModelDelegate);
        }

        public Task<TModel> SetModel<TModel>(TModel model)
        {
            var localModel = new SetModelClass<TModel>
                                 {
                                     Model = model
                                 };

            return this.ExecuteAsync(localModel, this.SetModelDelegate);
        }

        private Task<Model> GetModelDelegate(GetModelClass parameter, IInterfaceToImplement innerService)
        {
            return innerService.GetModel(parameter.intParameter, parameter.stringParameter);
        }

        private Task<TModel> SetModelDelegate<TModel>(SetModelClass<TModel> parameter, IInterfaceToImplement service)
        {
            return service.SetModel(parameter.Model);
        }
    }

    public class GetModelClass
    {
        public int intParameter;

        public string stringParameter;
    }

    public class SetModelClass<T>
    {
        public T Model;
    }
}