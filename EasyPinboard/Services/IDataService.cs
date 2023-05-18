using rm.Trie;

namespace EasyPinboard.Services
{
    public interface IDataService
    {
        Task InitializeDataStructures();
        Task LoadFromJson();
        Task<Dictionary<string, List<string>>> FindCategories();
        Task<Trie> FindTerms();
        Task<string> LoadJson(string url);
    }
}
