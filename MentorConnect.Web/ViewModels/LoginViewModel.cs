using System.ComponentModel.DataAnnotations;

namespace MentorConnect.Web.ViewModels;

public class LoginViewModel
{
    [Display(Name = "Email Address")]
    [Required(ErrorMessage = "Email address is required")]
    public string EmailAddress { get; init; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; init; } = string.Empty;
}