using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcApp.Models;
using Newtonsoft.Json.Linq;

namespace MvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult AnyProtectedPage()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // LogOut only from Mvc app.
            // To LogOut from Identity Server use: return SignOut("Cookies", "oidc");

            await HttpContext.SignOutAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CallWebApiApp()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = await client.GetStringAsync("https://localhost:6001/api/identity");

            ViewBag.Json = JArray.Parse(content).ToString();
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
