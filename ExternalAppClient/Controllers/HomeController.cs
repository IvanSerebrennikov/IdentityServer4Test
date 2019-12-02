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

        public async Task<IActionResult> GetTokenTest()
        {
            var client = new HttpClient();
            var identityServerUrl = "https://localhost:5001";
            var disco = await client.GetDiscoveryDocumentAsync(identityServerUrl);
            if (disco.IsError)
            {
                return Content(disco.Error);
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
                return Content(disco.Error);
            }

            _accessToken = tokenResponse.AccessToken;

            return Content(tokenResponse.Json.ToString());
        }

        public async Task<IActionResult> UseTokenTest()
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                return Content("Access Token is null");
            }

            // call api
            var client = new HttpClient();
            var apiUrl = "https://localhost:6001";
            client.SetBearerToken(_accessToken);

            var response = await client.GetAsync($"{apiUrl}/api/identity");
            if (!response.IsSuccessStatusCode)
            {
                return Content("Status code: " + response.StatusCode.ToString());
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(JArray.Parse(content).ToString());
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
