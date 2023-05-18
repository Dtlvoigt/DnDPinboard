using EasyPinboard.Models;
using EasyPinboard.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EasyPinboard.Controllers
{
    public class PinboardController : Controller
    {
        private readonly IDataService _data;
        private readonly ILogger<PinboardController> _logger;

        public PinboardController(IDataService data, ILogger<PinboardController> logger)
        {
            Console.WriteLine("controller!!");
            _data = data;
            _logger = logger;
            //var jsonBuilderService = new JsonBuilderService();
        }

        public IActionResult Index()
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
    }
}