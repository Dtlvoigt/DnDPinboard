using DnDPinboard.Models;
using DnDPinboard.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DnDPinboard.Controllers
{
    public class PinboardController : Controller
    {
        private static IDataService _data;
        private readonly ILogger<PinboardController> _logger;

        public PinboardController(IDataService data, ILogger<PinboardController> logger)
        {
            //var jsonBuilderService = new JsonBuilderService();
            _data = data;
            _logger = logger;
        }

        public IActionResult Pinboard()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }




        [HttpGet]
        public async Task<IActionResult> AutoComplete(string prefix)
        {
            List<string> suggestions = await _data.GetAutoCorrectSuggestions(prefix);
            return Json(suggestions);
        }

        //private async Task<List<string>> GetAutoCompleteSuggestions(string prefix)
        //{
        //    List<string> suggestions = new List<string>();
        //    suggestions = await _data.GetAutoCorrectSuggestions(prefix);

        //    //var items = _data  //TrieNode node = trie.root;
        //    //foreach (char c in prefix)
        //    //{
        //    //    if (node.Children.ContainsKey(c))
        //    //    {
        //    //        node = node.Children[c];
        //    //    }
        //    //    else
        //    //    {
        //    //        // Prefix not found, return empty suggestions
        //    //        return suggestions;
        //    //    }
        //    //}

        //    //// Collect all words with the given prefix using depth-first search
        //    //CollectWords(node, prefix, suggestions);

        //    return suggestions;
        //}

        //private void CollectWords(TrieNode node, string prefix, List<string> suggestions)
        //{
        //    if (node.IsWord)
        //    {
        //        suggestions.Add(prefix);
        //    }

        //    //foreach (char c in node.Children.Keys)
        //    //{
        //    //    CollectWords(node.Children[c], prefix + c, suggestions);
        //    //}
        //}
    }
}