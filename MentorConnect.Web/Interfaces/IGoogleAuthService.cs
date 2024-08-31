using MentorConnect.Web.Services.Results;
using Microsoft.AspNetCore.Authentication;

namespace MentorConnect.Web.Interfaces;

public interface IGoogleAuthService
{
    Task<GoogleAuthResult> HandleGoogleResponseAsync(AuthenticateResult authenticateResult);
}