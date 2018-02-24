namespace InterfaceCloneAndAddWithDebug.Interfaces
{
    public interface IMyOtherGenericInterface<T> : IMyOtherInterface
    {
        T ALotOfGeneric(T genericTypeParam);
    }
}