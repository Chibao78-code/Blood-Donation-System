using Microsoft.AspNetCore.Mvc;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.DTOs;
using System.Text.Json;

namespace BloodDonation.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAuthService authService, ILogger<AccountController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.LoginAsync(model);

        if (result.Success)
        {
            HttpContext.Session.SetString("UserId", result.User.Id.ToString());
            HttpContext.Session.SetString("Username", result.User.Username);
            HttpContext.Session.SetString("Role", result.User.Role);
            
            _logger.LogInformation($"User {result.User.Username} logged in at {DateTime.Now}");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            switch (result.User.Role.ToLower())
            {
                case "admin":
                    return RedirectToAction("Dashboard", "Admin");
                case "medicalcenter":
                    return RedirectToAction("Index", "MedicalCenter");  
                case "staff":
                    return RedirectToAction("Index", "Staff");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }

        ModelState.AddModelError("", result.Message);
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.RegisterAsync(model);

        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Login");
        }

        ModelState.AddModelError("", result.Message);
        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> CheckUsername(string username)
    {
        if(string.IsNullOrEmpty(username))
            return Json(true);
            
        var exists = await _authService.UserExistsAsync(username);
        return Json(!exists);
    }

    [HttpGet]
    public async Task<IActionResult> CheckEmail(string email)
    {
        if(string.IsNullOrEmpty(email))
            return Json(true);
            
        var exists = await _authService.EmailExistsAsync(email);
        return Json(!exists);
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
