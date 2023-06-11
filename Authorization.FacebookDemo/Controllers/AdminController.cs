using System.Security.Claims;
using Authorization.FacebookDemo.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.FacebookDemo.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "Administrator")]
        public IActionResult Administrator()
        {
            return View();
        }

        [Authorize(Policy = "Manager")]
        public IActionResult Manager()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var externalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();

            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalProviders = externalProviders
            };

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUri = Url.Action(nameof(ExternalLoginCallback), "Admin", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUri);
            return Challenge(properties, provider);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

            if (!result.Succeeded)
            {
                var externalLoginViewModel = new ExternalLoginViewModel
                {
                    UserName = info.Principal.FindFirstValue(ClaimTypes.Name)!,
                    ReturnUrl = returnUrl
                };

                return RedirectToAction(nameof(RegisterExternal), externalLoginViewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public IActionResult RegisterExternal(ExternalLoginViewModel model)
        {
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ActionName(nameof(RegisterExternal))]
        public async Task<IActionResult> RegisterExternalConfirmed(ExternalLoginViewModel model)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var user = new ApplicationUser(model.UserName);

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, string.Join(",", createResult.Errors));
                return View(model);
            }

            var claims = new List<Claim>
            {
                new("Demo", "Value"),
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Role, "Administrator")
            };

            var claimsResult = await _userManager.AddClaimsAsync(user, claims);

            if (!claimsResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, string.Join(",", claimsResult.Errors));
                return View(model);
            }

            var addLoginResult = await _userManager.AddLoginAsync(user, info);

            if (!addLoginResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, string.Join(",", addLoginResult.Errors));
                return View(model);
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "User not found");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Wrong password");
                return View(model);
            }

            //var claims = new List<Claim>
            //{
            //    new("Demo", "Value"),
            //    new(ClaimTypes.Name, model.UserName),
            //    new(ClaimTypes.Role, "Administrator")
            //};
            //var claimsIdentity = new ClaimsIdentity(claims, "Cookie");
            //var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            //await HttpContext.SignInAsync("Cookie", claimsPrincipal);

            return Redirect(model.ReturnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}
