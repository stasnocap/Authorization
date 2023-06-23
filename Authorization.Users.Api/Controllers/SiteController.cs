using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Users.Api.Controllers
{
    public class SiteController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SiteController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetOrdersAsync()
        {
            var authClient = _httpClientFactory.CreateClient();
            var discoveryDocument = await authClient.GetDiscoveryDocumentAsync("https://localhost:10001");


            var ordersClient = _httpClientFactory.CreateClient();

            var message = await ordersClient.GetStringAsync("https://localhost:7284/Site/GetSecrets");

            ViewData["Message"] = message;

            return View();
        }
    }
}
