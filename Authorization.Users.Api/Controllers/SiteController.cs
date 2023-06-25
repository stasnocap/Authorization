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

            var tokenResonse = await authClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "client_id",
                ClientSecret = "client_secret",
                Scope = "OrdersAPI",
            });

            var ordersClient = _httpClientFactory.CreateClient();

            ordersClient.SetBearerToken(tokenResonse.AccessToken);

            var getSecretsResponse = await ordersClient.GetAsync("https://localhost:7284/Site/GetSecrets");

            if (!getSecretsResponse.IsSuccessStatusCode)
            {
                ViewData["Message"] = getSecretsResponse.StatusCode.ToString();
                return View();
            }

            ViewData["Message"] = await getSecretsResponse.Content.ReadAsStringAsync();

            return View();
        }
    }
}
