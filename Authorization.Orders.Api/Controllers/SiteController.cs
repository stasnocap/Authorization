using Microsoft.AspNetCore.Mvc;

namespace Authorization.Orders.Api.Controllers
{
    public class SiteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public string GetSecrets()
        {
            return "Secret string from Orders.Api";
        }
    }
}
