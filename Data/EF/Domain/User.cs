using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Data.EF.Domain;

public class User: IdentityUser<long>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public virtual ICollection<ChatUser> ChatUsers { get; set; }
    public virtual ICollection<ChatMessage> ChatMessages { get; set; }
}