using System.Collections.Generic;

namespace WebApplication2.Handlers.Auth;

public class RegisterResponse
{
    public ICollection<string> Errors { get; set; }
}