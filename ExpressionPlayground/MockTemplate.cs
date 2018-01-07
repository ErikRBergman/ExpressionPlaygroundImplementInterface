// ReSharper disable StyleCop.SA1307
// ReSharper disable StyleCop.SA1401
namespace ExpressionPlayground
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using ExpressionPlayground.Test;
    using ExpressionPlayground.Test.Interfaces;

    public class MockTemplate : ProxyBase<ITestInterface>, ITestInterface
    {
        public MockTemplate(ITestInterface inner)
            : base(inner)
        {
        }

        public KeyValuePair<KeyValuePair<T1, KeyValuePair<T2, T3>>, KeyValuePair<T2, KeyValuePair<T3, T2>>> ComplexGenericStructure<T1, T2, T3>(
            KeyValuePair<KeyValuePair<T1, KeyValuePair<T2, T3>>, KeyValuePair<T2, KeyValuePair<T3, T2>>> parameter)
        {
            return this.Execute(
                new ComplexGenericStructure_Closure<T1, T2, T3>
                    {
                        parameter = parameter
                    },
                this.ComplexGenericStructure_Delegate);
        }

        public KeyValuePair<KeyValuePair<T1, KeyValuePair<T2, T3>>, KeyValuePair<T2, KeyValuePair<T3, T2>>> ComplexGenericStructure_Delegate<T1, T2, T3>(
            ComplexGenericStructure_Closure<T1, T2, T3> parameter,
            ITestInterface service)
        {
            return service.ComplexGenericStructure(parameter.parameter);
        }

        public TModel GenericResult_GenericParameter<TModel>(TModel model)
        {
            return this.Execute(model, this.GenericResult_GenericParameter_Delegate);
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

        // public Task<T1> GenericsAndVarArgs<T1>()
        ////public Task<T1> GenericsAndVarArgs<T1>(params T1[] parameters)
        // {
        // return this.ExecuteAsync(s => this.GenericsAndVarArgs_delegate<T1>(s));
        // }
        public Task<T1> GenericsAndVarArgs<T1>(T1[] parameters)
        {
            return this.ExecuteAsync(
                new GenericsAndVarArgs_Closure<T1>
                    {
                        parameters = parameters
                    },
                this.GenericsAndVarArgs_delegate);
        }

        public Task<T1> GenericsAndVarArgs_delegate<T1>(GenericsAndVarArgs_Closure<T1> parameters, ITestInterface innerService)
        {
            return innerService.GenericsAndVarArgs(parameters.parameters);
        }

        public void NoResult_Generic_NoParameters<T1>()
        {
            throw new NotImplementedException();
        }

        // public Task<T1> GenericsAndVarArgs_delegate<T1>(IInterfaceToImplement innerService)
        ////public Task<T1> GenericsAndVarArgs_delegate<T1>(GenericsAndVarArgs_Closure<T1> modelClass, IInterfaceToImplement innerService)
        // {
        // //return innerService.GenericsAndVarArgs(modelClass.parameters);
        // return innerService.GenericsAndVarArgs<T1>();
        // }
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

            return this.ExecuteAsync(localModel, this.Result_GenericParameter_Async_Delegate);
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

            return this.ExecuteAsync(getModel, this.Result_Parameters_Async_Delegate);
        }

        public Model Result_Parameters_delegate(GetModelClass modelClass, ITestInterface innerService)
        {
            return innerService.Result_Parameters(modelClass.intParameter, modelClass.stringParameter);
        }

        private Task<Model> Result_Parameters_Async_Delegate(GetModelClass parameter, ITestInterface innerService)
        {
            return innerService.Result_Parameters_Async(parameter.intParameter, parameter.stringParameter);
        }

        private Task<int> Result_GenericParameter_Async_Delegate<TModel>(SetModelClass<TModel> parameter, ITestInterface service)
        {
            return service.Result_GenericParameter_Async(parameter.Model);
        }

        private TModel GenericResult_GenericParameter_Delegate<TModel>(TModel m, ITestInterface service)
        {
            return m;
        }

        private Task<TModel> GenericResultGenericParameterAsyncDelegate<TModel>(SetModelClass<TModel> parameter, ITestInterface service)
        {
            return service.GenericResult_GenericParameter_Async(parameter.Model);
        }

        private Task NoResult_NoParameters_Delegate(ITestInterface innerService)
        {
            return innerService.NoResult_NoParameters_Async();
        }

        public class ComplexGenericStructure_Closure<T1, T2, T3>
        {
            public KeyValuePair<KeyValuePair<T1, KeyValuePair<T2, T3>>, KeyValuePair<T2, KeyValuePair<T3, T2>>> parameter;
        }

        public class GenericsAndVarArgs_Closure<T1>
        {
            public T1[] parameters;
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