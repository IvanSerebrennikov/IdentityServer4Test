using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ExternalAppClient.Models;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace ExternalAppClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // ONLY FOR TEST
        private static string _accessToken;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> GetToken()
        {
            var client = new HttpClient();
            var identityServerUrl = "https://localhost:5001";
            var disco = await client.GetDiscoveryDocumentAsync(identityServerUrl);
            if (disco.IsError)
            {
                ViewBag.WebApiResult = disco.Error;
                return View("Index");
            }

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "externalApp1",
                ClientSecret = "externalApp1secret",
                Scope = "apiOne"
            });

            if (tokenResponse.IsError)
            {
                ViewBag.WebApiResult = tokenResponse.Error;
                return View("Index");
            }

            _accessToken = tokenResponse.AccessToken;

            ViewBag.WebApiResult = tokenResponse.Json.ToString();
            return View("Index");
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
            if (string.IsNullOrEmpty(_accessToken))
            {
                ViewBag.WebApiResult = "Access Token is null";
                return View("Index");
            }

            // call api
            using var client = new HttpClient();
            var apiUrl = "https://localhost:6001";
            client.SetBearerToken(_accessToken);

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
