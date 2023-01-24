using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceReference;
using WebApplication2.Data.EF.Domain;
using WebApplication2.Models;

namespace WebApplication2.Controllers;

[Route("api/{controller}")]
public class LoginController: Controller
{
    public static Regex PhoneDetectRegex = new Regex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");
    
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public LoginController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet("Test")]
    public string Test()
    {
        return "lwjrth4th";
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModel request)
    {
        var user = await _userManager.FindByNameAsync(request.Login);

        if (user == null)
        {
            throw new Exception("");
        }

        var res = await _signInManager.PasswordSignInAsync(user, request.Password, true, true);

        if (!res.Succeeded)
        {
            return new JsonResult(new LoginResponse
            {
                Success = false
            });
        }
        
        return new JsonResult(new LoginResponse
        {
            Success = true
        });
    }
    
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel request)
    {
        var isPhone = PhoneDetectRegex.Match(request.MobileOrEmail).Success;

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = isPhone ? request.MobileOrEmail : null,
            Email = isPhone ? null : request.MobileOrEmail,
            UserName = request.Login
        };
        var res = await _userManager.CreateAsync(user, request.Password);

        if (!res.Succeeded)
        {
            return new JsonResult(new LoginResponse
            {
                Success = false,
                FieldErrors = res.Errors.Select(x => x.Description).ToArray()
            });
        }

        await _signInManager.PasswordSignInAsync(user, request.Password, true, true);

        return new JsonResult(new LoginResponse
        {
            Success = true
        });
    }
}

public class LoginResponse
{
    public bool Success { get; set; }
    public ICollection<string> FieldErrors { get; set; }
}
