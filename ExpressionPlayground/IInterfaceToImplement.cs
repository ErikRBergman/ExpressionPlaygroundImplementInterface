using System.Threading.Tasks;

namespace ExpressionPlayground
{
    public interface IInterfaceToImplement
    {
        Task<Model> GetModel(int intParameter, string stringParameter);

        Task<TModel> SetModel<TModel>(TModel model);
    }
}