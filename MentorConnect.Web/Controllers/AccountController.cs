using MentorConnect.Data.Entities;
using MentorConnect.Shared.Enums;
using MentorConnect.Shared.Exceptions;
using MentorConnect.Web.Interfaces;
using MentorConnect.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace MentorConnect.Web.Controllers;

public class AccountController : Controller
{
    private readonly IEmailService _emailService;
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(
        IEmailService emailService,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<AccountController> logger)
    {
        _emailService = emailService;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _logger = logger;
    }
    
    //GET: /Account/Profile
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        ApplicationUser? user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction("Index", "Home");
        }

        ProfileViewModel response = new ProfileViewModel
        {
            Email = user.Email ?? throw new EntityNotFoundException<ApplicationUser>(),
            Username = user.UserName ?? throw new EntityNotFoundException<ApplicationUser>(),
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber
        };
        return View(response);
    }

    // GET: /Account/Login
    public IActionResult Login()
    {
        LoginViewModel response = new LoginViewModel();
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
        
        if (user.EmailConfirmed is false)
        {
            return RedirectToAction("SendVerificationCode", new { email = user.Email });
        }

        SignInResult result = await _signInManager.PasswordSignInAsync(
            user, model.Password, false, false);
        if (result.Succeeded)
        {
            return RedirectToAction("Profile", "Account");
        }

        TempData["Error"] = "Invalid login attempt.";
        return View(model);
    }

    // GET: /Account/Register
    public IActionResult Register()
    {
        RegisterViewModel response = new RegisterViewModel();
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
            bool roleExists = await _roleManager.RoleExistsAsync(UserRoles.User);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }

            await _userManager.AddToRoleAsync(user, UserRoles.User);
        }

        return RedirectToAction("Login", "Account");
    }
    
    // GET: /Account/SendVerificationCode
    [HttpGet]
    public async Task<IActionResult> SendVerificationCode(string email)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction("Index", "Home");
        }

        string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        if (user.Email is not null)
        {
            await _emailService.SendEmailAsync(user.Email, "Verification Code", code);
            TempData["Success"] = "Verification code sent successfully.";
            
            return RedirectToAction("VerifyEmail", new { email = user.Email });
        }
        
        TempData["Error"] = "Failed to send verification code.";
        return RedirectToAction("Index", "Home");
    }
    
    // GET: /Account/VerifyEmail
    [HttpGet]
    public IActionResult VerifyEmail(string email)
    {
        VerifyEmailViewModel response = new VerifyEmailViewModel { Email = email };
        return View(response);
    }
    
    // POST: /Account/VerifyEmail
    [HttpPost]
    public async Task<IActionResult> VerifyEmail(string email, string code)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction("Index", "Home");
        }

        IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            TempData["Success"] = "Email verified successfully.";
            await _signInManager.SignInAsync(user, isPersistent: false);
            
            return RedirectToAction("Privacy", "Home");
        }

        TempData["Error"] = "Failed to verify email.";
        return RedirectToAction("Index", "Home");
    }
    
    // GET: /Account/ChangePassword
    [Authorize]
    public async Task<IActionResult> AddPassword()
    {
        ApplicationUser? user = await _userManager.GetUserAsync(User);
        if (user is null) 
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction("Index", "Home");
        }

        _logger.LogInformation($"User: {user.Id} found");
        ChangePasswordViewModel response = new ChangePasswordViewModel { AppUserId = user.Id };
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
            _logger.LogInformation($"Password added for user: {user.Id}");
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
        if (user is null)
        {
            return Json(false);
        }
        
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