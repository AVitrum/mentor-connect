namespace MentorConnect.Web.ViewModels;

public class ProfileViewModel
{
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required bool EmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
}