using Microsoft.AspNetCore.Mvc;

namespace Authorization.FacebookDemo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var identity = User.Identity;
            if (identity is not null)
            {
                ViewBag.IsAuthenticated = identity.IsAuthenticated;
                
                if (identity.Name is not null)
                {
                    ViewBag.Name = identity.Name;
                }
            }

            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
