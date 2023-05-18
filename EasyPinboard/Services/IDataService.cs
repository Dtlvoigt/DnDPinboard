
namespace DnDPinboard.Services
{
    public interface IDataService
    {
        Task InitializeDataStructures();
        Task LoadJsonItems();
        Task<List<string>> GetAutoCorrectSuggestions(string prefix);
        Task<string> LoadJson(string url);
    }
}
