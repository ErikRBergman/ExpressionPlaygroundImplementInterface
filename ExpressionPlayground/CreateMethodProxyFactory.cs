using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionPlayground
{
    using System.Linq.Expressions;
    using System.Reflection;

    using Microsoft.ServiceFabric.Actors.Remoting.V1.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;

    using Xunit;

    public class CreateMethodProxyFactory
    {
        public static ActorMethodDispatcherBase GetOrCreateMethodDispatcher(Type type)
        {
            return new MockMethodDispatcher();
        }

        public static Delegate CreateMethodProxy(MethodInfo methodInfo, Type returnValueType, Type parameterType, string parameterName = null)
        {
            var typeParameter = Expression.Parameter(parameterType, parameterName);
            return Expression.Lambda(typeof(Func<,>).MakeGenericType(parameterType, returnValueType), Expression.Call(methodInfo, typeParameter), typeParameter).Compile();
        }

        public static Func<TParameterType, TReturnValue> CreateMethodProxy<TParameterType, TReturnValue>(MethodInfo methodInfo, string parameterName = null)
        {
            return (Func<TParameterType, TReturnValue>)CreateMethodProxy(methodInfo, typeof(TReturnValue), typeof(TParameterType), parameterName);
        }

        public class TheBase
        {
            public static Guid Guid { get; } = Guid.NewGuid();
        }

        public class Class1 : TheBase
        {
            
        }

        public class Class2 : TheBase
        {
            
        }


        [Fact]
        public void CreateProxyTest()
        {
            var c1 = Class1.Guid;
            var c2 = Class2.Guid;



            Expression<Func<Type, MethodDispatcherBase>> originalExpression = type => GetOrCreateMethodDispatcher(type);

            var codeBuilderType = typeof(CreateMethodProxyFactory).Assembly.GetType("ExpressionPlayground.CreateMethodProxyFactory");
            var getOrCreateMethodDispatcher = codeBuilderType?.GetMethod("GetOrCreateMethodDispatcher", BindingFlags.Public | BindingFlags.Static);

            if (getOrCreateMethodDispatcher == null)
            {
                throw new Exception("wtf");
            }

            var typeParameter = Expression.Parameter(typeof(Type), "type");
            var newExpression = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(Type), typeof(MethodDispatcherBase)), Expression.Call(getOrCreateMethodDispatcher, typeParameter), typeParameter).Compile();

            var newExpressionFromFunc = CreateMethodProxy<Type, MethodDispatcherBase>(getOrCreateMethodDispatcher, "type");
            var newExpressionInstance = newExpressionFromFunc(typeof(Type));

            var x = newExpression;

            //var value = newExpression(typeof(CreateMethodProxyFactory));

            //    var codeBuilderType =
            //    typeof(Microsoft.ServiceFabric.Actors.Client.ActorProxyFactory)?.Assembly.GetType("Microsoft.ServiceFabric.Actors.Remoting.V1.Builder.ActorCodeBuilder");

            //var getOrCreateMethodDispatcher = codeBuilderType?.GetMethod("GetOrCreateMethodDispatcher", BindingFlags.Public | BindingFlags.Static);
            //var methodDispatcherBase = getOrCreateMethodDispatcher?.Invoke(null, new object[] { actorInterfaceType }) as MethodDispatcherBase;


        }

        //public void CreateMethodProxy()
    }
}
