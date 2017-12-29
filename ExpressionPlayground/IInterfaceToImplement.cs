namespace ExpressionPlayground
{
    using System.Threading.Tasks;

    public interface IInterfaceToImplement
    {
        Task<Model> GetModel(int intParameter, string stringParameter);

        Task<TModel> SetModel<TModel>(TModel model);
    }
}