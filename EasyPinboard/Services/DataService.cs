using DnDPinboard.Models;
using rm.Trie;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DnDPinboard.Services
{
    public class DataService : IDataService
    {
        private static Dictionary<string, List<string>>? _searchCategories;
        private static Dictionary<string, List<string>>? _itemCategories;
        private static Trie _searchTerms;

        /* The purpose of DataService is to create an abstraction between models and the actual database.
           Other classes should not have direct access to the database and must go through DataService. */
        public DataService()
        {
            InitializeDataStructures().GetAwaiter().GetResult();
        }

        public async Task InitializeDataStructures()
        {
            if (_searchCategories == null || _searchTerms == null || _itemCategories == null)
            {
                _itemCategories = new Dictionary<string, List<string>>();
                _searchCategories = new Dictionary<string, List<string>>();
                _searchTerms = new Trie();
                await LoadJsonItems();
            }
        }

        //fills data structures with items from local json files
        public async Task LoadJsonItems()
        {
            string path = "JSON Data";
            string[] fileNames = Directory.GetFiles(path);
            int count = 0;

            if (fileNames.Length == 0)
            {
                return;
            }

            //iterate through each json file in the directory, adding terms + categories to dictionary
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
                        string name = result.name.ToLower();
                        itemList.Add(name);

                        //if dictionary already has this key, add the category onto the corresponding list, else create a new key
                        if (_itemCategories.ContainsKey(name))
                        {
                            _itemCategories[name].Add(category);
                        }
                        else
                        {
                            var newList = new List<string>();
                            newList.Add(category);
                            _itemCategories.Add(name, newList);
                        }

                        _searchTerms.AddWord(name);
                        count++;
                    }
                }

                if (itemList != null)
                {
                    _searchCategories.Add(category, itemList);
                }
            }
        }

        //get auto correct suggestions for a certain prefix from the Trie
        public async Task<List<string>> GetAutoCorrectSuggestions(string prefix)
        {
            var suggestions = _searchTerms.GetWords(prefix).ToList();
            var itemsWithCategories = await AddCategories(suggestions);
            return itemsWithCategories;
        }

        public async Task<List<string>> AddCategories(List<string> items)
        {
            var itemsWithCategories = new List<string>();
            foreach (string item in items)
            {
                foreach (var category in _itemCategories[item])
                {
                    string updatedItem = item + " (" + category + ")";
                    itemsWithCategories.Add(updatedItem);
                }
                //string updatedItem = item + " (" + _itemCategories[item] + ")";
                //itemsWithCategories.Add(updatedItem);
            }

            return itemsWithCategories;
        }

        public async Task<string> CallAPI(string url)
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
