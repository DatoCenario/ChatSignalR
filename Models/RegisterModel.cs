using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models;

public class RegisterModel
{
    [Required] 
    public string FirstName { get; set; }

    [Required] 
    public string LastName { get; set; }
    
    [Required] 
    public string MobileOrEmail { get; set; }

    [Required] 
    public string Login { get; set; }
    
    [Required] 
    [DataType(DataType.Password)]
    public string Password { get; set; }
}