namespace ExpressionPlayground
{
    using System.Threading.Tasks;

    public class MockTemplate : ProxyBase<IInterfaceToImplement>, IInterfaceToImplement
    {
        public MockTemplate(IInterfaceToImplement inner)
            : base(inner)
        {
        }

        public Task<Model> A(int intParameter, string stringParameter)
        {
            var getModel = new GetModelClass
                               {
                                   intParameter = intParameter,
                                   stringParameter = stringParameter
                               };

            return this.ExecuteAsync(getModel, this.a_delegate);
        }

        public Task<int> B<TModel>(TModel model)
        {
            var localModel = new SetModelClass<TModel>
                                 {
                                     Model = model
                                 };

            return this.ExecuteAsync(localModel, this.b_delegate);
        }

        public Task<TModel> C<TModel>(TModel model)
        {
            var localModel = new SetModelClass<TModel>
                                 {
                                     Model = model
                                 };

            return this.ExecuteAsync(localModel, this.c_delegate);
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

        private Task<Model> a_delegate(GetModelClass parameter, IInterfaceToImplement innerService)
        {
            return innerService.A(parameter.intParameter, parameter.stringParameter);
        }

        private Task<TModel> c_delegate<TModel>(SetModelClass<TModel> parameter, IInterfaceToImplement service)
        {
            return service.C(parameter.Model);
        }

        private Task<int> b_delegate<TModel>(SetModelClass<TModel> parameter, IInterfaceToImplement service)
        {
            return service.B(parameter.Model);
        }
    }

    public struct GetModelClass
    {
        public int intParameter;

        public string stringParameter;
    }

    public class SetModelClass<T>
    {
        public T Model;
    }
}