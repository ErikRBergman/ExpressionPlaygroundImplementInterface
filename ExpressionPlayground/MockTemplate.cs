﻿namespace ExpressionPlayground
{
    using System;
    using System.Threading.Tasks;

    public class MockTemplate : ProxyBase<IInterfaceToImplement>, IInterfaceToImplement
    {
        public MockTemplate(IInterfaceToImplement inner)
            : base(inner)
        {
        }

        public TModel GenericResult_GenericParameter<TModel>(TModel model)
        {
            return this.Execute(model, this.GenericResult_GenericParameter_Delegate);
        }

        private TModel GenericResult_GenericParameter_Delegate<TModel>(TModel m, IInterfaceToImplement service)
        {
            return m;
        }

        public Task<TModel> GenericResult_GenericParameter_Async<TModel>(TModel model)
        {
            var localModel = new SetModelClass<TModel>
                                 {
                                     Model = model
                                 };

            return this.ExecuteAsync(localModel, this.GenericResultGenericParameterAsyncDelegate);
        }

        public ThreeGenericParameter<T1, T2, T3> GenericResult_GenericParameters<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            throw new NotImplementedException();
        }

        public Task<ThreeGenericParameter<T1, T2, T3>> GenericResult_GenericParameters_Async<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            return Task.FromResult(
                new ThreeGenericParameter<T1, T2, T3>
                    {
                        t1 = t1,

                        t2 = t2,

                        t3 = t3
                    });
        }

        public void NoResult_Generic_NoParameters<T1>()
        {
            throw new NotImplementedException();
        }

        public Task NoResult_Generic_NoParameters_Async<T1>()
        {
            return Task.CompletedTask;
        }

        public void NoResult_Generic_Parameters<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            throw new NotImplementedException();
        }

        public Task NoResult_Generic_Parameters_Async<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            return Task.CompletedTask;
        }

        public void NoResult_NoParameters()
        {
            throw new NotImplementedException();
        }

        public Task NoResult_NoParameters_Async()
        {
            return this.ExecuteAsync(this.NoResult_NoParameters_Delegate);
        }

        public void NoResult_Parameters(int intParameter, string stringParameter)
        {
            throw new NotImplementedException();
        }

        public Task NoResult_Parameters_Async(int intParameter, string stringParameter)
        {
            return Task.CompletedTask;
        }

        public int Result_Generic_NoParameters<T1>()
        {
            throw new NotImplementedException();
        }

        public Task<int> Result_Generic_NoParameters_Async<T1>()
        {
            throw new NotImplementedException();
        }

        public int Result_GenericParameter<TModel>(TModel model)
        {
            throw new NotImplementedException();
        }

        public Task<int> Result_GenericParameter_Async<TModel>(TModel model)
        {
            var localModel = new SetModelClass<TModel>
                                 {
                                     Model = model
                                 };

            return this.ExecuteAsync(localModel, this.b_delegate);
        }

        public int Result_NoParameters()
        {
            throw new NotImplementedException();
        }

        public Task<int> Result_NoParameters_Async()
        {
            throw new NotImplementedException();
        }

        public Model Result_Parameters(int intParameter, string stringParameter)
        {
            return this.Execute(
                new GetModelClass
                    {
                        intParameter = intParameter,
                        stringParameter = stringParameter
                    },
                this.Result_Parameters_delegate);
        }

        public Task<Model> Result_Parameters_Async(int intParameter, string stringParameter)
        {
            var getModel = new GetModelClass
                               {
                                   intParameter = intParameter,
                                   stringParameter = stringParameter
                               };

            return this.ExecuteAsync(getModel, this.a_delegate);
        }

        public Model Result_Parameters_delegate(GetModelClass modelClass, IInterfaceToImplement innerService)
        {
            return innerService.Result_Parameters(modelClass.intParameter, modelClass.stringParameter);
        }

        private Task<Model> a_delegate(GetModelClass parameter, IInterfaceToImplement innerService)
        {
            return innerService.Result_Parameters_Async(parameter.intParameter, parameter.stringParameter);
        }

        private Task<int> b_delegate<TModel>(SetModelClass<TModel> parameter, IInterfaceToImplement service)
        {
            return service.Result_GenericParameter_Async(parameter.Model);
        }

        private Task<TModel> GenericResultGenericParameterAsyncDelegate<TModel>(SetModelClass<TModel> parameter, IInterfaceToImplement service)
        {
            return service.GenericResult_GenericParameter_Async(parameter.Model);
        }

        private Task NoResult_NoParameters_Delegate(IInterfaceToImplement innerService)
        {
            return innerService.NoResult_NoParameters_Async();
        }
    }

    public class GetModelClass
    {
        public int intParameter;

        public string stringParameter;
    }

    public sealed class SetModelClass<T>
    {
        public T Model;
    }
}