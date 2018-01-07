namespace ExpressionPlayground
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;

    public class TestImplementation : IInterfaceToImplement
    {
        private readonly List<TestMethodCall> methodCalls = new List<TestMethodCall>();

        public IEnumerable<TestMethodCall> TestMethodCalls => this.methodCalls;

        public void ClearMethodCalls() => this.methodCalls.Clear();

        public Task<TModel> GenericResult_GenericParameter_Async<TModel>(TModel model)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.GenericResult_GenericParameter_Async), model));
            return Task.FromResult(model);
        }

        public Task<int> GenericResult_GenericParameter<T1>()
        {
            var type = typeof(T1);
            this.methodCalls.Add(new TestMethodCall(nameof(this.GenericResult_GenericParameter_Async), type));
            return Task.FromResult(4321);
        }

        public Task<int> Result_NoParameters_Async()
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.Result_NoParameters_Async)));
            return Task.FromResult(4321);
        }

        public Task<ThreeGenericParameter<T1, T2, T3>> GenericResult_GenericParameters_Async<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.GenericResult_GenericParameters_Async), t1, t2, t3));

            return Task.FromResult(
                new ThreeGenericParameter<T1, T2, T3>
                {
                    t1 = t1,

                    t2 = t2,

                    t3 = t3
                });
        }

        public Task NoResult_Generic_NoParameters_Async<T1>()
        {
            var type = typeof(T1);
            this.methodCalls.Add(new TestMethodCall(nameof(this.NoResult_Generic_NoParameters_Async), type));
            return Task.CompletedTask;
        }

        public Model Result_Parameters(int intParameter, string stringParameter)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.Result_Parameters), intParameter, stringParameter));
            return new Model
            {
                IntValue = intParameter,
                StringValue = stringParameter
            };
        }

        public int Result_GenericParameter<TModel>(TModel model)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.Result_GenericParameter), typeof(TModel), model));
            return 123;
        }

        public TModel GenericResult_GenericParameter<TModel>(TModel model)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.GenericResult_GenericParameter), typeof(TModel), model));
            return model;
        }

        public int Result_Generic_NoParameters<T1>()
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.Result_Generic_NoParameters), typeof(T1)));
            return 123;
        }

        public int Result_NoParameters()
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.Result_NoParameters)));
            return 123;
        }

        public ThreeGenericParameter<T1, T2, T3> GenericResult_GenericParameters<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.GenericResult_GenericParameters), typeof(T1), t1, typeof(T2), t2, typeof(T3), t3));
            return new ThreeGenericParameter<T1, T2, T3>
            {
                t2 = t2,
                t1 = t1,
                t3 = t3
            };
        }

        public void NoResult_Parameters(int intParameter, string stringParameter)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.NoResult_Parameters), intParameter, stringParameter));
        }

        public void NoResult_Generic_Parameters<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.NoResult_Generic_Parameters), typeof(T1), t1, typeof(T2), t2, typeof(T3), t3));
        }

        public void NoResult_NoParameters()
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.NoResult_NoParameters)));
        }

        public void NoResult_Generic_NoParameters<T1>()
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.NoResult_Generic_NoParameters), typeof(T1)));
        }

        public Task<T1> GenericsAndVarArgs<T1>()
        //public Task<T1> GenericsAndVarArgs<T1>(T1[] parameters)
        //public Task<T1> GenericsAndVarArgs<T1>(params T1[] parameters)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.GenericsAndVarArgs), ImmutableList<object>.Empty.Add(typeof(T1))));
            //this.methodCalls.Add(new TestMethodCall(nameof(this.GenericsAndVarArgs), ImmutableList<object>.Empty.Add(typeof(T1)).AddRange(parameters.Select(p => (object)p))));
            return Task.FromResult<T1>(default(T1));
        }

        public Task<T1> GenericsAndVarArgs<T1>(T1[] parameters)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.GenericsAndVarArgs), ImmutableList<object>.Empty.Add(typeof(T1))));
            //this.methodCalls.Add(new TestMethodCall(nameof(this.GenericsAndVarArgs), ImmutableList<object>.Empty.Add(typeof(T1)).AddRange(parameters.Select(p => (object)p))));
            return Task.FromResult<T1>(default(T1));
        }

        public Task NoResult_Generic_Parameters_Async<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.NoResult_Generic_Parameters_Async), t1, t2, t3));
            return Task.CompletedTask;
        }

        public Task NoResult_NoParameters_Async()
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.NoResult_NoParameters_Async)));
            return Task.CompletedTask;
        }

        public Task NoResult_Parameters_Async(int intParameter, string stringParameter)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.NoResult_Parameters_Async), intParameter, stringParameter));
            return Task.CompletedTask;
        }

        public Task<int> Result_Generic_NoParameters_Async<T1>()
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.Result_Generic_NoParameters_Async), typeof(T1)));
            return Task.FromResult(321);
        }

        public Task<int> Result_GenericParameter_Async<TModel>(TModel model)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.Result_GenericParameter_Async), model));
            return Task.FromResult(123);
        }

        public Task<Model> Result_Parameters_Async(int intParameter, string stringParameter)
        {
            this.methodCalls.Add(new TestMethodCall(nameof(this.Result_Parameters_Async), intParameter, stringParameter));

            return Task.FromResult(
                new Model
                {
                    IntValue = intParameter,
                    StringValue = stringParameter
                });
        }
    }
}