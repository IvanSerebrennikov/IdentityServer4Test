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

        public async Task<IActionResult> AccessWebApiIdentity()
        {
            return await AccessWebApi("identity");
        }

        public async Task<IActionResult> AccessWebApiResourceOne()
        {
            return await AccessWebApi("resources/resource-one");
        }

        public async Task<IActionResult> AccessWebApiResourceTwo()
        {
            return await AccessWebApi("resources/resource-two");
        }

        private async Task<IActionResult> AccessWebApi(string accessUrl)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (string.IsNullOrEmpty(accessToken))
            {
                ViewBag.WebApiResult = "Access Token is null";
                return View("Index");
            }

            // call api
            using var client = new HttpClient();
            var apiUrl = "https://localhost:6001";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync($"{apiUrl}/api/{accessUrl}");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.WebApiResult = $"Response is not OK. StatusCode: {response.StatusCode}";
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                ViewBag.WebApiResult = JToken.Parse(content).ToString(Newtonsoft.Json.Formatting.Indented);
            }

            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
