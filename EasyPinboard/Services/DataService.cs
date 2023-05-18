using DnDPinboard.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DnDPinboard.Services
{
    public class DataService : IDataService
    {
        private static Dictionary<string, List<string>>? _searchCategories;
        private static Trie _searchTerms;

        /* The purpose of DataService is to create an abstraction between models and the actual database.
           Other classes should not have direct access to the database and must go through DataService. */
        public DataService() 
        {
            InitializeDataStructures().GetAwaiter().GetResult();
        }

        public async Task InitializeDataStructures()
        {
            if (_searchCategories == null || _searchTerms == null)
            {
                _searchCategories = new Dictionary<string, List<string>>();
                _searchTerms = new Trie();
                await LoadJsonItems();
            }
        }

        public async Task LoadJsonItems()
        {
            string path = "JSON Data";
            string[] fileNames = Directory.GetFiles(path);
            int count = 0;

            if (fileNames.Length > 0)
            {
                foreach (string file in fileNames)
                {
                    string category = Path.GetFileNameWithoutExtension(file);
                    List<string> itemList = new List<string>();
                    string jsonData = File.ReadAllText(file);

                    if (!string.IsNullOrEmpty(jsonData))
                    {
                        ResultsAPI data = JsonSerializer.Deserialize<ResultsAPI>(jsonData);
                        foreach (ResultsAPI.Result result in data.results)
                        {
                            itemList.Add(result.name.ToLower());
                            _searchTerms.Insert(result.name.ToLower());
                            count++;
                        }
                    }

                    if (itemList != null)
                    {
                        _searchCategories.Add(category, itemList);
                    }
                }
            }
        }

        //get auto correct suggestions for a certain prefix from the Trie
        public async Task<List<string>> GetAutoCorrectSuggestions(string prefix)
        {
            var suggestions = _searchTerms.GetAllWordsWithPrefix(prefix).ToList();
            return suggestions;
        }

        public async Task<string> LoadJson(string url)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            string json = "";
            try
            {
                json = await client.GetStringAsync("https://www.dnd5eapi.co/api/" + url);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return json;
        }
    }
}
