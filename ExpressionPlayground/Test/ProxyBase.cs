// ReSharper disable UnusedMember.Global
// ReSharper disable StyleCop.SA1409
// ReSharper disable RedundantEmptyFinallyBlock
namespace ExpressionPlayground.Test
{
    using System;
    using System.Threading.Tasks;

    public class ProxyBase<TInterface>
    {
        private readonly TInterface inner;

        public ProxyBase(TInterface inner)
        {
            this.inner = inner;
        }

        protected void Execute(Action<TInterface> action)
        {
            try
            {
                action(this.inner);
            }
            finally
            {
            }
        }

        protected TResult Execute<TResult>(Func<TInterface, TResult> func)
        {
            try
            {
                return func(this.inner);
            }
            finally
            {
            }
        }

        protected void Execute<TParameter>(TParameter parameter, Action<TParameter, TInterface> action)
        {
            try
            {
                action(parameter, this.inner);
            }
            finally
            {
            }
        }

        protected TResult Execute<TParameter, TResult>(TParameter parameter, Func<TParameter, TInterface, TResult> func)
        {
            try
            {
                return func(parameter, this.inner);
            }
            finally
            {
            }
        }

        protected async Task ExecuteAsync(Func<TInterface, Task> func)
        {
            try
            {
                await func(this.inner);
            }
            finally
            {
            }
        }

        protected async Task<TResult> ExecuteAsync<TResult>(Func<TInterface, Task<TResult>> func)
        {
            try
            {
                return await func(this.inner);
            }
            finally
            {
            }
        }

        protected async Task ExecuteAsync<TParameter>(TParameter parameter, Func<TParameter, TInterface, Task> func)
        {
            try
            {
                await func(parameter, this.inner);
            }
            finally
            {
            }
        }

        protected async Task<TResult> ExecuteAsync<TParameter, TResult>(TParameter parameter, Func<TParameter, TInterface, Task<TResult>> func)
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