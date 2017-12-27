namespace ExpressionPlayground
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Xunit;

    public class CreateInstanceFactory
    {
        public interface IMyInterface
        {
        }

        public static Delegate CreateInstance(Type typeToInstantiate, Type typeToReturn, params Type[] parameterTypes)
        {
            var constructorInfo = typeToInstantiate.GetConstructors().FirstOrDefault(
                c =>
                    {
                        var constructorParameters = c.GetParameters();
                        var parameterTypeCount = parameterTypes.Length;

                        if (parameterTypeCount != constructorParameters.Length)
                        {
                            return false;
                        }

                        for (var i = 0; i < parameterTypeCount; i++)
                        {
                            if (constructorParameters[i].ParameterType != parameterTypes[i])
                            {
                                return false;
                            }
                        }

                        return true;
                    });

            if (constructorInfo == null)
            {
                throw new Exception("Type " + typeToInstantiate.FullName + " does not contain a matching constructor.");
            }

            var currentExpression = Expression.Lambda(typeof(Func<>).MakeGenericType(typeToReturn), Expression.New(constructorInfo));
            return currentExpression.Compile();
        }


        public static Delegate CreateInstance(Type typeToInstantiate, Type typeToReturn)
        {
            var constructorInfo = typeToInstantiate.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 0);

            if (constructorInfo == null)
            {
                throw new Exception("Type " + typeToInstantiate.FullName + " does not contain a parameterless constructor.");
            }

            var currentExpression = Expression.Lambda(typeof(Func<>).MakeGenericType(typeToReturn), Expression.New(constructorInfo));
            return currentExpression.Compile();
        }

        public static Delegate CreateInstance(Type typeToInstantiate) => CreateInstance(typeToInstantiate, typeToInstantiate);

        public static Func<T> CreateInstance<T>()
        {
            return (Func<T>)CreateInstance(typeof(T));
        }

        public static Func<TTypeToReturn> CreateInstance<TTypeToInstantiate, TTypeToReturn>()
        {
            return (Func<TTypeToReturn>)CreateInstance(typeof(TTypeToInstantiate), typeof(TTypeToReturn));
        }

        public static Func<TTypeToReturn> CreateInstance<TTypeToInstantiate, TTypeToReturn>(params Type[] parameterTypes)
        {
            return (Func<TTypeToReturn>)CreateInstance(typeof(TTypeToInstantiate), typeof(TTypeToReturn), parameterTypes);
        }

        [Fact]
        public void TestsCreateInstance()
        {
            Expression<Func<MyInnerClass>> expression = () => new MyInnerClass();
            var createInstance = CreateInstance<MyInnerClass>();
            var createInterfaceInstance = CreateInstance<MyInnerClass, IMyInterface>();

            var o1 = createInstance();

            var o2 = createInterfaceInstance();

            Expression<Func<MyInnerClass>> nullInstanceExpression = () => (MyInnerClass)null;


            var secondConstructorFactory = CreateInstance<MyInnerClass, IMyInterface>(typeof(int), typeof(string), typeof(bool));


        }

        private class MyInnerClass : IMyInterface
        {
            public int X { get; set; }

            public string Y { get; set; }

            public bool Z { get; set; }

            public MyInnerClass()
            {

            }

            public MyInnerClass(int x, string y, bool z)
            {

            }

        }
    }
}