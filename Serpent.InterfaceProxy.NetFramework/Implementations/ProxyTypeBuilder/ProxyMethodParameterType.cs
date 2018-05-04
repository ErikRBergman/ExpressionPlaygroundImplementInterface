namespace Serpent.InterfaceProxy.Implementations.ProxyTypeBuilder
{
    public enum ProxyMethodParameterType
    {
        /// <summary>
        /// Auto detect is currently ignored
        /// </summary>
        AutoDetect = 0,

        ParametersClosure = 100,

        MethodDelegate = 200,

        MethodName = 10000,

        MethodCallContext = 11000

        //        MethodInfo = 11000
    }
}