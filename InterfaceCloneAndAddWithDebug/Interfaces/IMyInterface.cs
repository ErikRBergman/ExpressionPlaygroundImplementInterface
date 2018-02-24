using System.Threading.Tasks;
using InterfaceCloneAndAddWithDebug.Models;

namespace InterfaceCloneAndAddWithDebug.Interfaces
{
    public interface IMyInterface : IActor, IRubbish
    {
        Task<MyModel> DoMyModelAsync(MyModel model);
        Task<MyModel> DoMyModelNoArgumentsAsync();

        Task<MyModel> DoMyModelParamsAsync(int value, string text);
    }
}