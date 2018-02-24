namespace InterfaceCloneAndAddWithDebug.Interfaces
{
    public interface IMyGenericInterface<T> : IMyOtherInterface, IMyOtherGenericInterface<T>
    {
        T GetTypeGeneric(T genericTypeParam);

        TI GetGeneric<TI>(TI param);
    }
}