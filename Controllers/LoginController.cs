using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceReference;
using WebApplication2.Data.EF.Domain;
using WebApplication2.Models;

namespace WebApplication2.Controllers;

[Route("{controller}")]
public class LoginController: Controller
{
    private readonly Regex _phoneDetectRegex = new Regex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");
    
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public LoginController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet("Login")]
    public IActionResult Login()
    {
        return View("Login");
    }
    
    [HttpGet("Register")]
    public IActionResult Register()
    {
        return View("Register");
    }
    
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromForm] LoginModel request)
    {
        var user = await _userManager.FindByNameAsync(request.Login);

        if (user == null)
        {
            throw new Exception("");
        }

        var res = await _signInManager.PasswordSignInAsync(user, request.Password, true, true);

        if (!res.Succeeded)
        {
            return View("Login", request);
        }

        return RedirectToAction("Index", "Chat");
    }
    
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromForm] RegisterModel request)
    {
        var isPhone = _phoneDetectRegex.Match(request.MobileOrEmail).Success;

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
            ViewData.Add("RegisterErrors", res.Errors.Select(e => e.Description).ToArray());
            return View("Register", request);
        }

        await _signInManager.PasswordSignInAsync(user, request.Password, true, true);
        
        return RedirectToAction("Index", "Chat");
    }
}