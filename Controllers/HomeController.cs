using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MarketMap.Models;

namespace MarketMap.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Landing()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "\"MarketMap\" is a unique application which helps people who want to facilitate their shopping.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Main office";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
