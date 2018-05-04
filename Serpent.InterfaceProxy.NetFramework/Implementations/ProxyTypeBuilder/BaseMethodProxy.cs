namespace Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder
{
    using System;
    using System.Threading.Tasks;

    public class BaseMethodProxy<TInterface>
    {
        private readonly TInterface inner;

        public BaseMethodProxy(TInterface inner)
        {
            this.inner = inner;
        }

        protected TInterface InnerReference => this.inner;

        [ProxyMethod]
        protected virtual void Execute(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Action<TInterface> action)
        {
            action(this.inner);
        }

        [ProxyMethod]
        protected virtual TResult Execute<TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TInterface, TResult> func)
        {
            return func(this.inner);
        }

        [ProxyMethod]
        protected virtual void Execute<TParameter>(
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)]
            TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Action<TParameter, TInterface> action)
        {
            action(parameter, this.inner);
        }

        [ProxyMethod]
        protected virtual TResult Execute<TParameter, TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)]
            TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TParameter, TInterface, TResult> func)
        {
            return func(parameter, this.inner);
        }

        [ProxyMethod]
        protected virtual Task ExecuteAsync(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TInterface, Task> func)
        {
            return func(this.inner);
        }

        [ProxyMethod]
        protected virtual Task<TResult> ExecuteAsync<TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TInterface, Task<TResult>> func)
        {
            return func(this.inner);
        }

        [ProxyMethod]
        protected virtual Task ExecuteAsync<TParameter>(
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)]
            TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TParameter, TInterface, Task> func)
        {
            return func(parameter, this.inner);
        }

        [ProxyMethod]
        protected virtual Task<TResult> ExecuteAsync<TParameter, TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)]
            TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TParameter, TInterface, Task<TResult>> func)
        {
            return func(parameter, this.inner);
        }
    }
}