using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MentorConnect.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MentorConnect.Web.ApiControllers;

[ApiController]
[Route("server/[controller]")]
public class GoogleAuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public GoogleAuthController(
        IConfiguration configuration,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _configuration = configuration;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return new ChallengeResult(
            GoogleDefaults.AuthenticationScheme,
            new AuthenticationProperties
            {
                RedirectUri = "/server/googleAuth/googleResponse"
            });
    }

    //TODO: Move logic to a service
    [HttpGet("googleResponse")]
    public async Task<IActionResult> GoogleResponse()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded)
        {
            return BadRequest();
        }

        var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
        if (email is null)
        {
            return BadRequest("Email claim not found");
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            var newUser = new ApplicationUser
            {
                Email = email,
                UserName = email,
                EmailConfirmed = true,
                NormalizedEmail = email.ToUpper(),
                NormalizedUserName = email.ToUpper(),
            };
            
            await _userManager.CreateAsync(newUser);
            
            user = await _userManager.FindByEmailAsync(email);
            await _signInManager.SignInAsync(user!, isPersistent: false);
            
            return RedirectToAction("AddPassword", "Account");
        }

        if (user.PasswordHash is null)
        {
            await _signInManager.SignInAsync(user!, isPersistent: false);
            return RedirectToAction("AddPassword", "Account");
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
            
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:IssuerSigningKey"] ?? string.Empty));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        Response.Cookies.Append("jwt", tokenString, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Privacy", "Home");
    }
}