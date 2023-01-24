using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebApplication2.Controllers;
using WebApplication2.Data.EF.Domain;

namespace WebApplication2.Handlers.Auth;

public class RegisterQueryHandler: IRequestHandler<RegisterRequest, RegisterResponse>
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public RegisterQueryHandler(SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<RegisterResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var isPhone = LoginController.PhoneDetectRegex.Match(request.MobileOrEmail).Success;

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
            return new RegisterResponse()
            {
                Errors = res.Errors.Select(e => e.Description).ToList()
            };
        }

        return new RegisterResponse();
    }
}