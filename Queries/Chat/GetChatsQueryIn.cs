namespace WebApplication2.Queries.Chat;

public class GetChatsQueryIn
{
    public string? NameFilter { get; set; }
    public bool? OnlyUserChats { get; set; }
}