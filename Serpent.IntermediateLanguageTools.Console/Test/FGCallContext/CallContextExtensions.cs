namespace FG.CallContext
{
    public static class CallContextExtensions
    {
        public static void FreeNamedDataSlot(this CallContext callContext, string key)
        {
            callContext.RemoveItem(key);
        }

        public static void LogicalSetData(this CallContext callContext, string key, object value)
        {
            callContext.SetItem(key, value);
        }

        public static object LogicalGetData(this CallContext callContext, string key)
        {
            return callContext.GetItem(key);
        }

        public static T Get<T>(this CallContext callContext, string key)
        {
            return (T)callContext.GetItem(key);
        }
    }
}