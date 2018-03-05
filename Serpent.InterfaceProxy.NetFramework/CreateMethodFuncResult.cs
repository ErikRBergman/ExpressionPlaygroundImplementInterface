namespace Serpent.InterfaceProxy
{
    public struct CreateMethodFuncResult<TMethodContext>
    {
        public CreateMethodFuncResult(CreateMethodData createMethodData, TMethodContext methodContext)
        {
            this.CreateMethodData = createMethodData;
            this.MethodContext = methodContext;
        }

        public CreateMethodData CreateMethodData { get; set; }

        public TMethodContext MethodContext { get; set; }
    }
}