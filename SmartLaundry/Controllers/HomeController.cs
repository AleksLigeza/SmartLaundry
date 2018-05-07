using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartLaundry.Models;

namespace SmartLaundry.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Error(string message, string title)
        {
            var model = new ErrorViewModel
            {
                Message = message,
                Title = title
            };
            return View(model);
        }
    }
}