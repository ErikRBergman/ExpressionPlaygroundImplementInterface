namespace Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder
{
    using System;
    using System.Threading.Tasks;

    public class BaseMethodProxyWithMethodNames<TInterface>
    {
        public BaseMethodProxyWithMethodNames(TInterface inner)
        {
            this.InnerReference = inner;
        }

        protected TInterface InnerReference { get; }

        [ProxyMethod]
        protected virtual void Execute(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Action<TInterface> action)
        {
            action(this.InnerReference);
        }

        [ProxyMethod]
        protected virtual TResult Execute<TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TInterface, TResult> func)
        {
            return func(this.InnerReference);
        }

        [ProxyMethod]
        protected virtual void Execute<TParameter>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)]
            TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Action<TParameter, TInterface> action)
        {
            action(parameter, this.InnerReference);
        }

        [ProxyMethod]
        protected virtual TResult Execute<TParameter, TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)]
            TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TParameter, TInterface, TResult> func)
        {
            return func(parameter, this.InnerReference);
        }

        [ProxyMethod]
        protected virtual Task ExecuteAsync(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TInterface, Task> func)
        {
            return func(this.InnerReference);
        }

        [ProxyMethod]
        protected virtual Task<TResult> ExecuteAsync<TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TInterface, Task<TResult>> func)
        {
            return func(this.InnerReference);
        }

        [ProxyMethod]
        protected virtual Task ExecuteAsync<TParameter>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)]
            TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TParameter, TInterface, Task> func)
        {
            return func(parameter, this.InnerReference);
        }

        [ProxyMethod]
        protected virtual Task<TResult> ExecuteAsync<TParameter, TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)]
            TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TParameter, TInterface, Task<TResult>> func)
        {
            return func(parameter, this.InnerReference);
        }
    }
}