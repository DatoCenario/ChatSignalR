using WebApplication2.Handlers.Auth;

namespace WebApplication2.Queries.Auth;

public class RegisterQueryIn: RegisterRequest
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string MobileOrEmail { get; set; }
    
    public string Login { get; set; }

    public string Password { get; set; }
}