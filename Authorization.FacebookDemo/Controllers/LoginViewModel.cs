using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace Authorization.FacebookDemo.Controllers;

public class LoginViewModel
{
    [Required]
    public string UserName { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    public string ReturnUrl { get; set; } = null!;

    public IEnumerable<AuthenticationScheme> ExternalProviders { get; set; } = null!;
}