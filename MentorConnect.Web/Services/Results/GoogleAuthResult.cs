namespace MentorConnect.Web.Services.Results;

public class GoogleAuthResult
{
    public bool Success { get; }
    public string? Token { get; }
    public bool NeedsPassword { get; }
    public string? ErrorMessage { get; }

    private GoogleAuthResult(bool success, string? token, bool needsPassword, string? errorMessage)
    {
        Success = success;
        Token = token;
        NeedsPassword = needsPassword;
        ErrorMessage = errorMessage;
    }

    public static GoogleAuthResult SuccessResult(string token) => new(true, token, false, null);
    public static GoogleAuthResult PasswordNeeded() => new(true, null, true, null);
    public static GoogleAuthResult Failure(string errorMessage) => new(false, null, false, errorMessage);
}