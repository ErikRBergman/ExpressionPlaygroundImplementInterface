namespace Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder
{
    using System;
    using System.Threading.Tasks;

    public class BaseMethodProxyWithMethodNames<T1, T2>
    {
        public BaseMethodProxyWithMethodNames(T1 innerType1, T2 innerType2)
        {
            this.InnerType1Reference = innerType1;
            this.InnerType2Reference = innerType2;
        }

        protected T1 InnerType1Reference { get; }

        protected T2 InnerType2Reference { get; }

        [ProxyMethod]
        protected virtual void Execute(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Action<T1> action)
        {
            action(this.InnerType1Reference);
        }

        [ProxyMethod]
        protected virtual TResult Execute<TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<T1, TResult> func)
        {
            return func(this.InnerType1Reference);
        }

        [ProxyMethod]
        protected virtual void Execute<TParameter>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)]
            TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Action<TParameter, T1> action)
        {
            action(parameter, this.InnerType1Reference);
        }

        [ProxyMethod]
        protected virtual TResult Execute<TParameter, TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)]
            TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TParameter, T1, TResult> func)
        {
            return func(parameter, this.InnerType1Reference);
        }

        [ProxyMethod]
        protected virtual void Execute(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Action<T2> action)
        {
            action(this.InnerType2Reference);
        }

        [ProxyMethod]
        protected virtual TResult Execute<TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<T2, TResult> func)
        {
            return func(this.InnerType2Reference);
        }

        [ProxyMethod]
        protected virtual void Execute<TParameter>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)]
            TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Action<TParameter, T2> action)
        {
            action(parameter, this.InnerType2Reference);
        }

        [ProxyMethod]
        protected virtual TResult Execute<TParameter, TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)]
            TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TParameter, T2, TResult> func)
        {
            return func(parameter, this.InnerType2Reference);
        }

        [ProxyMethod]
        protected virtual Task ExecuteAsync(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<T1, Task> func)
        {
            return func(this.InnerType1Reference);
        }

        [ProxyMethod]
        protected virtual Task<TResult> ExecuteAsync<TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<T1, Task<TResult>> func)
        {
            return func(this.InnerType1Reference);
        }

        [ProxyMethod]
        protected virtual Task ExecuteAsync<TParameter>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)]
            TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TParameter, T1, Task> func)
        {
            return func(parameter, this.InnerType1Reference);
        }

        [ProxyMethod]
        protected virtual Task<TResult> ExecuteAsync<TParameter, TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)]
            TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TParameter, T1, Task<TResult>> func)
        {
            return func(parameter, this.InnerType1Reference);
        }

        [ProxyMethod]
        protected virtual Task ExecuteAsync(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<T2, Task> func)
        {
            return func(this.InnerType2Reference);
        }

        [ProxyMethod]
        protected virtual Task<TResult> ExecuteAsync<TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<T2, Task<TResult>> func)
        {
            return func(this.InnerType2Reference);
        }

        [ProxyMethod]
        protected virtual Task ExecuteAsync<TParameter>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)]
            TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TParameter, T2, Task> func)
        {
            return func(parameter, this.InnerType2Reference);
        }

        [ProxyMethod]
        protected virtual Task<TResult> ExecuteAsync<TParameter, TResult>(
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodName)]
            string methodName,
            [ProxyMethodParameterType(ProxyMethodParameterType.ParametersClosure)]
            TParameter parameter,
            [ProxyMethodParameterType(ProxyMethodParameterType.MethodDelegate)]
            Func<TParameter, T2, Task<TResult>> func)
        {
            return func(parameter, this.InnerType2Reference);
        }
    }
}