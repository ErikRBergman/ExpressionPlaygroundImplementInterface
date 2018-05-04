// ReSharper disable UnusedMember.Global
// ReSharper disable StyleCop.SA1409
// ReSharper disable RedundantEmptyFinallyBlock
namespace Serpent.InterfaceProxy.NetFramework.Tests.Test
{
    using System;
    using System.Threading.Tasks;

    using Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder;

    public class ProxyBase<TInterface>
    {
        private readonly TInterface inner;

        public ProxyBase(TInterface inner)
        {
            this.inner = inner;
        }

        [ProxyMethod]
        protected void Execute(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Action<TInterface> action)
        {
            try
            {
                action(this.inner);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected TResult Execute<TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)] string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TInterface, TResult> func)
        {

            try
            {
                return func(this.inner);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected void Execute<TParameter>(
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)] TParameter parameter, 
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]Action<TParameter, TInterface> action)
        {
            try
            {
                action(parameter, this.inner);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected TResult Execute<TParameter, TResult>([ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)] TParameter parameter, [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]Func<TParameter, TInterface, TResult> func)
        {
            try
            {
                return func(parameter, this.inner);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected async Task ExecuteAsync([ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TInterface, Task> func)
        {
            try
            {
                await func(this.inner);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected async Task<TResult> ExecuteAsync<TResult>([ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TInterface, Task<TResult>> func)
        {
            try
            {
                return await func(this.inner);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected async Task ExecuteAsync<TParameter>(
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)] TParameter parameter, 
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TParameter, TInterface, Task> func)
        {
            try
            {
                await func(parameter, this.inner);
            }
            finally
            {
            }
        }

        [ProxyMethod]
        protected async Task<TResult> ExecuteAsync<TParameter, TResult>([ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)] TParameter parameter, [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]Func<TParameter, TInterface, Task<TResult>> func)
        {
            try
            {
                return await func(parameter, this.inner);
            }
            finally
            {
            }
        }
    }
}