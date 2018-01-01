namespace ExpressionPlayground
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