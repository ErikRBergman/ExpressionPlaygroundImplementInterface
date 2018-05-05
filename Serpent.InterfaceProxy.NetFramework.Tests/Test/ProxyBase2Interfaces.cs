// ReSharper disable UnusedMember.Global
// ReSharper disable StyleCop.SA1409
// ReSharper disable RedundantEmptyFinallyBlock
namespace Serpent.InterfaceProxy.NetFramework.Tests.Test
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder;

    public interface IInterface1
    {
        Task<string> Method1Async(string prefix);

        Task<string> WithDefault(CancellationToken token = default(CancellationToken));
    }

    public interface IInterface3
    {
        Task<IEnumerable<Guid>> ListCommandsAsync();
    }

    public interface IInterface2 : IInterface3
    {
        Task<string> Method2Async(string prefix);

    }

    public class Class1 : IInterface1
    {
        public string LastPrefix { get; private set; }

        public async Task<string> Method1Async(string prefix)
        {
            this.LastPrefix = prefix;
            return prefix + nameof(this.Method1Async);
        }

        public async Task<string> WithDefault(CancellationToken token = default(CancellationToken))
        {
            return null;
        }
    }

    public class Class2 : IInterface2
    {
        public string LastPrefix { get; private set; }

        public async Task<string> Method2Async(string prefix)
        {
            this.LastPrefix = prefix;
            return prefix + nameof(this.Method2Async);
        }

        public Task<IEnumerable<Guid>> ListCommandsAsync()
        {
            return Task.FromResult<IEnumerable<Guid>>(null);
        }
    }

    public class ProxyBase<TI1, TI2>
    {
        private readonly TI1 innerInterface1;

        private readonly TI2 innerInterface2;

        public ProxyBase(TI1 innerInterface1, TI2 innerInterface2)
        {
            this.innerInterface1 = innerInterface1;
            this.innerInterface2 = innerInterface2;
        }

        [ProxyMethod]
        protected void Execute(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Action<TI1> action)
        {
            try
            {
                action(this.innerInterface1);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected TResult Execute<TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)] string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.TypeName)] string typeName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TI1, TResult> func)
        {

            try
            {
                return func(this.innerInterface1);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected void Execute<TParameter>(
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)] TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]Action<TParameter, TI1> action)
        {
            try
            {
                action(parameter, this.innerInterface1);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected TResult Execute<TParameter, TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)] TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TParameter, TI1, TResult> func)
        {
            try
            {
                return func(parameter, this.innerInterface1);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected async Task ExecuteAsync(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TI1, Task> func)
        {
            try
            {
                await func(this.innerInterface1);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected async Task<TResult> ExecuteAsync<TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TI1, Task<TResult>> func)
        {
            try
            {
                return await func(this.innerInterface1);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected async Task ExecuteAsync<TParameter>(
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)] TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TParameter, TI1, Task> func)
        {
            try
            {
                await func(parameter, this.innerInterface1);
            }
            finally
            {
            }
        }

        public ConcurrentBag<string> Methodcalls { get; } = new ConcurrentBag<string>();

        [ProxyMethod]
        protected async Task<TResult> ExecuteAsync<TParameter, TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)] string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.TypeName)] string typeName,
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)] TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TParameter, TI1, Task<TResult>> func)
        {
            this.Methodcalls.Add($"{typeName}.{methodName}");

            try
            {
                return await func(parameter, this.innerInterface1);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected void Execute(
          [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Action<TI2> action)
        {
            try
            {
                action(this.innerInterface2);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected TResult Execute<TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)] string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.TypeName)] string typeName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TI2, TResult> func)
        {

            try
            {
                return func(this.innerInterface2);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected void Execute<TParameter>(
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)] TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]Action<TParameter, TI2> action)
        {
            try
            {
                action(parameter, this.innerInterface2);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected TResult Execute<TParameter, TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)] TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TParameter, TI2, TResult> func)
        {
            try
            {
                return func(parameter, this.innerInterface2);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected async Task ExecuteAsync(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TI2, Task> func)
        {
            try
            {
                await func(this.innerInterface2);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected async Task<TResult> ExecuteAsync<TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TI2, Task<TResult>> func)
        {
            try
            {
                return await func(this.innerInterface2);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected async Task ExecuteAsync<TParameter>(
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)] TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TParameter, TI2, Task> func)
        {
            try
            {
                await func(parameter, this.innerInterface2);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected async Task<TResult> ExecuteAsync<TParameter, TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)] TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TParameter, TI2, Task<TResult>> func)
        {
            try
            {
                return await func(parameter, this.innerInterface2);
            }
            finally
            {
            }
        }
    }
}