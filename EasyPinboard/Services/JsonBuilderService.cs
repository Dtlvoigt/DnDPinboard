using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EasyPinboard.Services
{
    //this service will only be used manually to update local files from the API
    public class JsonBuilderService
    {
        public JsonBuilderService() 
        {
            ResetFiles();
            var categories = GetCategories();
            CreateItemFiles(categories);
        }

        public void ResetFiles()
        {
            string folder = "JSON Data";
            string[] files = Directory.GetFiles(folder);

            foreach(string path in files)
            {
                File.Delete(path);
            }
        }

        public List<string> GetCategories()
        {
            //load category data from api
            List<string> categoriesList = new List<string>();
            string categoriesJson = LoadJson("").GetAwaiter().GetResult();

            //create a list of category items to be loaded
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

        public void CreateItemFiles(List<string> categories)
        {
            string folder = "JSON Data";
            string filePath;

            foreach (string category in categories)
            {
                string itemsJson = LoadJson(category).GetAwaiter().GetResult();
                filePath = Path.Combine(folder, category + ".json");
                File.WriteAllText(filePath, itemsJson);
            }
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
