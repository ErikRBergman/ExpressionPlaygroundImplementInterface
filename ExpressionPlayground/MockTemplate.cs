namespace ExpressionPlayground
{
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;

    public class MockTemplate : ProxyBase<IInterfaceToImplement>, IInterfaceToImplement
    {
        public MockTemplate(IInterfaceToImplement inner)
            : base(inner)
        {
        }

        public Task<Model> Result_Parameters(int intParameter, string stringParameter)
        {
            var getModel = new GetModelClass
                               {
                                   intParameter = intParameter,
                                   stringParameter = stringParameter
                               };

            return this.ExecuteAsync(getModel, this.a_delegate);
        }

        public Task<int> Result_GenericParameter<TModel>(TModel model)
        {
            var localModel = new SetModelClass<TModel>
                                 {
                                     Model = model
                                 };

            return this.ExecuteAsync(localModel, this.b_delegate);
        }

        public Task<TModel> GenericResult_GenericParameter<TModel>(TModel model)
        {
            var localModel = new SetModelClass<TModel>
                                 {
                                     Model = model
                                 };

            return this.ExecuteAsync(localModel, this.c_delegate);
        }

        public Task<int> Result_Generic_NoParameters<T1>()
        {
            throw new System.NotImplementedException();
        }

        public Task<int> Result_NoParameters()
        {
            throw new System.NotImplementedException();
        }

        public Task<ThreeGenericParameter<T1, T2, T3>> GenericResult_GenericParameters<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            return Task.FromResult(
                new ThreeGenericParameter<T1, T2, T3>
                    {
                        t1 = t1,

                        t2 = t2,

                        t3 = t3
                    });
        }

        public Task NoResult_Parameters(int intParameter, string stringParameter)
        {
            return Task.CompletedTask;
        }

        public Task NoResult_Generic_Parameters<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            return Task.CompletedTask;
        }

        public Task NoResult_NoParameters()
        {
            return this.ExecuteAsync(this.NoResult_NoParameters_Delegate);
        }

        private Task NoResult_NoParameters_Delegate(IInterfaceToImplement innerService)
        {
            return innerService.NoResult_NoParameters();
        }

        public Task NoResult_Generic_NoParameters<T1>()
        {
            return Task.CompletedTask;
        }




        private Task<Model> a_delegate(GetModelClass parameter, IInterfaceToImplement innerService)
        {
            return innerService.Result_Parameters(parameter.intParameter, parameter.stringParameter);
        }

        private Task<TModel> c_delegate<TModel>(SetModelClass<TModel> parameter, IInterfaceToImplement service)
        {
            return service.GenericResult_GenericParameter(parameter.Model);
        }

        private Task<int> b_delegate<TModel>(SetModelClass<TModel> parameter, IInterfaceToImplement service)
        {
            return service.Result_GenericParameter(parameter.Model);
        }
    }

    public struct GetModelClass
    {
        public int intParameter;

        public string stringParameter;
    }

    public sealed class SetModelClass<T>
    {
        public T Model;
    }
}