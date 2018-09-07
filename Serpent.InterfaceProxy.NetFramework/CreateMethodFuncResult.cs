namespace Serpent.InterfaceProxy
{
    public struct CreateMethodFuncResult<TMethodContext>
    {
        public CreateMethodFuncResult(InterfaceProxyMethodInformation interfaceProxyMethodInformation, TMethodContext methodContext)
        {
            this.InterfaceProxyMethodInformation = interfaceProxyMethodInformation;
            this.MethodContext = methodContext;
        }

        public InterfaceProxyMethodInformation InterfaceProxyMethodInformation { get; set; }

        public TMethodContext MethodContext { get; set; }
    }
}