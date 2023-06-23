using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authorization.JWTBearer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        public string Authenticate()
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, "Stas"),
                new Claim(JwtRegisteredClaimNames.Email, "stasnocap@gmail.com"),
            };

            var dateTimeNow = DateTime.Now;

            var secretBytes = Encoding.UTF8.GetBytes(Constants.SecretKey);
            var key = new SymmetricSecurityKey(secretBytes);

            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                Constants.Issuer, 
                Constants.Audience,
                claims,
                notBefore: dateTimeNow,
                expires: dateTimeNow.AddMinutes(60),
                signingCredentials);

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenStr;
        }
    }
}
