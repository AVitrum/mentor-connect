using MentorConnect.Web.Interfaces;
using MentorConnect.Web.Services.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MentorConnect.Web.ApiControllers;

[ApiController]
[Route("server/[controller]")]
public class GoogleAuthController : ControllerBase
{
    private readonly IGoogleAuthService _googleAuthService;

    public GoogleAuthController(IGoogleAuthService googleAuthService)
    {
        _googleAuthService = googleAuthService;
    }
    
    public IActionResult Index()
    {
        return new ChallengeResult(
            GoogleDefaults.AuthenticationScheme,
            new AuthenticationProperties
            {
                RedirectUri = "/server/GoogleAuth/google-response"
            });
    }

    [HttpGet("google-response")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GoogleResponse()
    {
        AuthenticateResult authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        GoogleAuthResult result = await _googleAuthService.HandleGoogleResponseAsync(authenticateResult);

        if (!result.Success)
        {
            return Unauthorized(result.ErrorMessage);
        }

        if (result.NeedsPassword)
        {
            return RedirectToAction("AddPassword", "Account");
        }

        Response.Cookies.Append("jwt", result.Token!, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
        return RedirectToAction("Privacy", "Home");
    }
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("test")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Test()
    {
        return Ok("You are authorized");
    }
}