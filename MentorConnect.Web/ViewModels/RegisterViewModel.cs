using System.ComponentModel.DataAnnotations;

namespace MentorConnect.Web.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [Display(Name = "Email address")]
    public string Email { get; init; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; init; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required")]
    [Display(Name = "Confirm password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; init; } = string.Empty;
}