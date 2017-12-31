namespace ExpressionPlayground
{
    using System.Threading.Tasks;

    public interface IInterfaceToImplement
    {
        Task<Model> A(int intParameter, string stringParameter);

        Task<int> B<TModel>(TModel model);


        Task<TModel> C<TModel>(TModel model);
    }
}