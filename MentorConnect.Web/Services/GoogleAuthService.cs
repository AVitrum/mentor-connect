using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MentorConnect.Data.Entities;
using MentorConnect.Web.Interfaces;
using MentorConnect.Web.Services.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MentorConnect.Web.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GoogleAuthService> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public GoogleAuthService(
        IConfiguration configuration,
        ILogger<GoogleAuthService> logger,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _configuration = configuration;
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<GoogleAuthResult> HandleGoogleResponseAsync(AuthenticateResult authenticateResult)
    {
        if (!authenticateResult.Succeeded)
        {
            return GoogleAuthResult.Failure("Authentication failed");
        }

        var email = GetEmailFromClaims(authenticateResult);
        if (email is null)
        {
            return GoogleAuthResult.Failure("Email claim not found");
        }

        var user = await GetUserByEmailAsync(email);
        if (user is null)
        {
            return await HandleNewUserAsync(email);
        }

        if (user.PasswordHash is null)
        {
            return await HandleUserWithoutPasswordAsync(user);
        }

        var token = GenerateJwtToken(user, email);
        await _signInManager.SignInAsync(user, isPersistent: false);
        
        return GoogleAuthResult.SuccessResult(token);
    }

    private string? GetEmailFromClaims(AuthenticateResult authenticateResult)
    {
        return authenticateResult.Principal?.FindFirst(ClaimTypes.Email)?.Value;
    }

    private async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    private async Task<GoogleAuthResult> HandleNewUserAsync(string email)
    {
        _logger.LogInformation("User not found, creating new user");
        var newUser = new ApplicationUser
        {
            Email = email,
            UserName = email,
            EmailConfirmed = true,
            NormalizedEmail = email.ToUpper(),
            NormalizedUserName = email.ToUpper(),
        };

        await _userManager.CreateAsync(newUser);
        var user = await _userManager.FindByEmailAsync(email);
        await _signInManager.SignInAsync(user!, isPersistent: false);
        return GoogleAuthResult.PasswordNeeded();
    }

    private async Task<GoogleAuthResult> HandleUserWithoutPasswordAsync(ApplicationUser user)
    {
        _logger.LogInformation("User has no password, redirecting to add password page");
        await _signInManager.SignInAsync(user, isPersistent: false);
        return GoogleAuthResult.PasswordNeeded();
    }

    private string GenerateJwtToken(ApplicationUser user, string email)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:IssuerSigningKey"] ?? string.Empty));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
