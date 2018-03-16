namespace StatelessService1.Interfaces
{
    using System.Threading.Tasks;

    public interface IStatelessService1 : IService
    {
        Task<ReturnDataModel> DoItAsync(RequestDataModel request, string text);

        Task<ReturnDataModel> NoParametersAsync();

        Task DoNothingAsync();
    }


    public interface IStatelessService1Clone : IService
    {
        Task<ReturnDataModel> DoItAsync(InsertedCorrelationData correlationData, RequestDataModel request, string text);

        Task<ReturnDataModel> NoParametersAsync(InsertedCorrelationData correlationData);

        Task DoNothingAsync(InsertedCorrelationData correlationData);

    }

    public interface IService
    {

    }
}