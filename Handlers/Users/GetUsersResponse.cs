using System.Collections.Generic;
using WebApplication2.Handlers.Interfaces;

namespace WebApplication2.Handlers.Users;

public class GetUsersResponse: IPaginationResponse
{
    public long EntitiesLeft { get; set; }
    
    public ICollection<UserResponse> Users { get; set; }
}

public class UserResponse
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}