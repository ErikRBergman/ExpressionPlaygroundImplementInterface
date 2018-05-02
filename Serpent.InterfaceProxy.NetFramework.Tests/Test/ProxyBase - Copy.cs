//// ReSharper disable UnusedMember.Global
//// ReSharper disable StyleCop.SA1409
//// ReSharper disable RedundantEmptyFinallyBlock
//namespace Serpent.InterfaceProxy.NetFramework.Tests.Test
//{
//    using System;
//    using System.Collections.Concurrent;
//    using System.Reflection;
//    using System.Threading.Tasks;

//    public interface ITypedLogger
//    {
//        void LogNothing();

//        void LogError(Exception e, string text);

//    }

//    public class SimpleImplementation
//    {
//        private readonly ConcurrentDictionary<MethodInfo, >

//        protected void Execute(MethodInfo method)
//        {
//        }

//        protected TResult Execute<TResult>(MethodInfo method)
//        {
//            return default(TResult);
//        }

//        protected void Execute<TParameter>(MethodInfo method, TParameter parameter)
//        {
//        }

//        protected TResult Execute<TParameter, TResult>(MethodInfo methodInfo, TParameter parameter)
//        {
//            return default(TResult);
//        }

//        protected async Task ExecuteAsync(MethodInfo method)
//        {
//        }

//        protected async Task<TResult> ExecuteAsync<TResult>(MethodInfo method)
//        {
//            return default(TResult);
//        }

//        protected async Task ExecuteAsync<TParameter>(MethodInfo method, TParameter parameter)
//        {
//        }

//        protected async Task<TResult> ExecuteAsync<TParameter, TResult>(MethodInfo method, TParameter parameter)
//        {
//            return default(TResult);
//        }
//    }
//}