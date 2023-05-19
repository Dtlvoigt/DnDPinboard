
namespace DnDPinboard.Services
{
    public interface IDataService
    {
        Task InitializeDataStructures();
        Task LoadJsonItems();
        Task<List<string>> GetAutoCorrectSuggestions(string prefix);
        Task<List<string>> AddCategories(List<string> items);
        Task<string> CallAPI(string url);
    }
}
