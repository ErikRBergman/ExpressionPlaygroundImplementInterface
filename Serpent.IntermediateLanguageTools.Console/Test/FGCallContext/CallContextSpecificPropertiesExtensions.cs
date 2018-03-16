namespace FG.CallContext
{
    public static class CallContextSpecificPropertiesExtensions
    {
        public static string CorrelationId(this CallContext callContext)
        {
            return callContext.GetItem(Constants.ExecutionTree.CorrelationIdPropertyName) as string;
        }

        public static CallContext CorrelationId(this CallContext callContext, string correlationId)
        {
            return callContext.SetItem(Constants.ExecutionTree.CorrelationIdPropertyName, correlationId);
        }
    }
}