using System.ComponentModel.DataAnnotations;

namespace Authorization.FacebookDemo.Controllers;

public class ExternalLoginViewModel
{
    [Required]
    public string UserName { get; set; } = null!;

    [Required]
    public string ReturnUrl { get; set; } = null!;
}