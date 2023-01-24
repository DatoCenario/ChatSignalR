namespace WebApplication2.Queries.Users;

public class GetUsersQueryIn
{
    public string? FullNameFilter { get; set; }
    public string? PhoneOrMailFilter { get; set; }
    public long[]? IdFilter { get; set; }
    public long[]? ChatIdFilter { get; set; }
}