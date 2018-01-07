namespace ExpressionPlayground
{
    public class TestMethodCall
    {
        public TestMethodCall(string methodName, params object[] parameters)
        {
            this.MethodName = methodName;
            this.Parameters = parameters;
        }

        public string MethodName { get; }

        public object[] Parameters { get; }
    }
}