namespace Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder
{
    using System;
    using System.Threading.Tasks;

    public class BaseMethodProxyWithMethodAndTypeNames<TInterface>
    {
        private readonly TInterface inner;

        public BaseMethodProxyWithMethodAndTypeNames(TInterface inner)
        {
            this.inner = inner;
        }

        protected TInterface InnerReference => this.inner;

        [ProxyMethod]
        protected virtual void Execute(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)] string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.TypeName)] string typeName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Action<TInterface> action)
        {
            action(this.inner);
        }

        [ProxyMethod]
        protected virtual TResult Execute<TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)] string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.TypeName)] string typeName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TInterface, TResult> func)
        {

            return func(this.inner);
        }

        [ProxyMethod]
        protected virtual void Execute<TParameter>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)] string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.TypeName)] string typeName,
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)] TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]Action<TParameter, TInterface> action)
        {
            action(parameter, this.inner);
        }

        [ProxyMethod]
        protected virtual TResult Execute<TParameter, TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)] string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.TypeName)] string typeName,
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)] TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]Func<TParameter, TInterface, TResult> func)
        {
            return func(parameter, this.inner);
        }

        [ProxyMethod]
        protected virtual Task ExecuteAsync(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)] string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.TypeName)] string typeName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TInterface, Task> func)
        {
            return func(this.inner);
        }

        [ProxyMethod]
        protected virtual Task<TResult> ExecuteAsync<TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)] string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.TypeName)] string typeName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TInterface, Task<TResult>> func)
        {
            return func(this.inner);
        }

        [ProxyMethod]
        protected virtual Task ExecuteAsync<TParameter>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)] string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.TypeName)] string typeName,
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)] TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)] Func<TParameter, TInterface, Task> func)
        {
            return func(parameter, this.inner);
        }

        [ProxyMethod]
        protected virtual Task<TResult> ExecuteAsync<TParameter, TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)] string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.TypeName)] string typeName,
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)] TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]Func<TParameter, TInterface, Task<TResult>> func)
        {
            return func(parameter, this.inner);
        }
    }
}
