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
        public static Dictionary<string, List<string>>? _searchCategories;
        public static Trie? _searchTerms;

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
                await LoadFromJson();
            }
        }

        public async Task LoadFromJson()
        {
            string path = "JSON Data";
            string[] fileNames = Directory.GetFiles(path);

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
                            _searchTerms.AddWord(result.name.ToLower());
                        }
                    }

                    if (itemList != null)
                    {
                        _searchCategories.Add(category, itemList);
                    }
                }
            }
        }

        //create a list of all major categories
        public async Task<List<string>> CategoryList()/////////////////////////////////////
        {
            List<string> categoriesList = new List<string>();
            string categoriesJson = await LoadJson("");

            if (!string.IsNullOrEmpty(categoriesJson))
            {
                JsonDocument jsonDocCategories = JsonDocument.Parse(categoriesJson);
                JsonElement rootCategories = jsonDocCategories.RootElement;

                foreach (JsonProperty category in rootCategories.EnumerateObject())
                {
                    if (!String.IsNullOrEmpty(category.Name))
                    {
                        categoriesList.Add(category.Name);
                    }
                }
            }

            return categoriesList;
        }

        //load item categories into data structure with sub terms
        public async Task<Dictionary<string, List<string>>> FindCategories()
        {
            var searchCategories = new Dictionary<string, List<string>>();
            _searchTerms = new Trie();
            List<string> categoriesList = await CategoryList();

            if(categoriesList != null)
            {
                foreach (string category in categoriesList)
                {
                    List<string> itemList = new List<string>();
                    string itemsJson = await LoadJson(category);
                    if (!String.IsNullOrEmpty(itemsJson))
                    {
                        ResultsAPI data = JsonSerializer.Deserialize<ResultsAPI>(itemsJson);
                        foreach(ResultsAPI.Result result in data.results)
                        {
                            itemList.Add(result.name);
                            _searchTerms.AddWord(result.name);
                        }
                    }

                    if(itemList != null)
                    {
                        searchCategories.Add(category, itemList);
                    }
                }
            }

            return searchCategories;
        }

        //load item keywords into Trie data structure for fast searching later
        public async Task<Trie> FindTerms()
        {
            Trie searchTerms = new Trie();


            return searchTerms;
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
