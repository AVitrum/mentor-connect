using MentorConnect.Data.Entities;
using MentorConnect.Shared.Enums;
using MentorConnect.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace MentorConnect.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }
    
    // GET: /Account/Login
    public IActionResult Login()
    {
        var response = new LoginViewModel();
        return View(response);
    }
    
    // POST: /Account/Login
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        ApplicationUser? user = await _userManager.FindByEmailAsync(model.EmailAddress);

        if (user is null)
        {
            TempData["Error"] = "Wrong email or password. Please try again.";
            return View(model);
        }
        
        bool passwordMatch = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!passwordMatch)
        {
            TempData["Error"] = "Wrong email or password. Please try again.";
            return View(model);
        }
        
        SignInResult result = await _signInManager.PasswordSignInAsync(
            user, model.Password, false, false);
        if (result.Succeeded)
        {
            //TODO: Redirect to the user's profile
            return RedirectToAction("Privacy", "Home");
        }
        
        TempData["Error"] = "Invalid login attempt.";
        return View(model);
    }
    
    // GET: /Account/Register
    public IActionResult Register()
    {
        var response = new RegisterViewModel();
        return View(response);
    }
    
    // POST: /Account/Register
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        ApplicationUser? user = await _userManager.FindByEmailAsync(model.Email);
        if (user is not null)
        {
            TempData["Error"] = "Email address is already in use.";
            return View(model);
        }

        user = new ApplicationUser
        {
            Email = model.Email,
            UserName = model.Email
        };
        IdentityResult creationResult = await _userManager.CreateAsync(user, model.Password);
        if (creationResult.Succeeded)
        {
            var roleExists = await _roleManager.RoleExistsAsync(UserRoles.User);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }
            
            await _userManager.AddToRoleAsync(user, UserRoles.User);
        }

        return RedirectToAction("Login", "Account");
    }

    // GET: /Account/ChangePassword
    [Authorize]
    public async Task<IActionResult> AddPassword()
    {
        ApplicationUser? user = await _userManager.GetUserAsync(User);
        var response = new ChangePasswordViewModel { AppUserId = user!.Id };
        return View(response);
    }
    
    // POST: /Account/ChangePassword
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddPassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        ApplicationUser? user = await _userManager.FindByIdAsync(model.AppUserId);
        if (user is null)
        {
            TempData["Error"] = "User not found.";
            return View(model);
        }

        IdentityResult result = await _userManager.AddPasswordAsync(user, model.Password);
        if (result.Succeeded)
        {
            TempData["Success"] = "Password added successfully.";
            return RedirectToAction("Index", "Home");
        }

        TempData["Error"] = "Failed to add password.";
        return View(model);
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> HasPassword()
    {
        ApplicationUser? user = await _userManager.GetUserAsync(User);
        bool hasPassword = await _userManager.HasPasswordAsync(user);
        return Json(hasPassword);
    }
    
    // GET: /Account/Logout
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}