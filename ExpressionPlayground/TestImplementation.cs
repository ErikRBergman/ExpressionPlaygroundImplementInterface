namespace ExpressionPlayground
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class TestImplementation : IInterfaceToImplement
    {
        private readonly List<TestMethodCall> methodCalls = new List<TestMethodCall>();

        public IEnumerable<TestMethodCall> TestMethodCalls => this.methodCalls;

        public void ClearMethodCalls() => this.methodCalls.Clear();

        public Task<TModel> GenericResult_GenericParameter<TModel>(TModel model)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.GenericResult_GenericParameter), model));
            return Task.FromResult(model);
        }

        public Task<int> GenericResult_GenericParameter<T1>()
        {
            var type = typeof(T1);
            this.methodCalls.Add(new TestMethodCall(nameof(this.GenericResult_GenericParameter), type));
            return Task.FromResult(4321);
        }

        public Task<int> Result_NoParameters()
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.Result_NoParameters)));
            return Task.FromResult(4321);
        }

        public Task<ThreeGenericParameter<T1, T2, T3>> GenericResult_GenericParameters<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.GenericResult_GenericParameters), t1, t2, t3));

            return Task.FromResult(
                new ThreeGenericParameter<T1, T2, T3>
                    {
                        t1 = t1,

                        t2 = t2,

                        t3 = t3
                    });
        }

        public Task NoResult_Generic_NoParameters<T1>()
        {
            var type = typeof(T1);
            this.methodCalls.Add(new TestMethodCall(nameof(this.NoResult_Generic_NoParameters), type));
            return Task.CompletedTask;
        }

        public Task NoResult_Generic_Parameters<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.NoResult_Generic_Parameters), t1, t2, t3));
            return Task.CompletedTask;
        }

        public Task NoResult_NoParameters()
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.NoResult_NoParameters)));
            return Task.CompletedTask;
        }

        public Task NoResult_Parameters(int intParameter, string stringParameter)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.NoResult_Parameters), intParameter, stringParameter));
            return Task.CompletedTask;
        }

        public Task<int> Result_Generic_NoParameters<T1>()
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.Result_Generic_NoParameters), typeof(T1)));
            return Task.FromResult(321);
        }

        public Task<int> Result_GenericParameter<TModel>(TModel model)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.Result_GenericParameter), model));
            return Task.FromResult(123);
        }

        public Task<Model> Result_Parameters(int intParameter, string stringParameter)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.Result_Parameters), intParameter, stringParameter));

            return Task.FromResult(
                new Model
                    {
                        IntValue = intParameter,
                        StringValue = stringParameter
                    });
        }
    }
}