namespace WebApplication2.CommonModels;

public class CreateChatEventArgs
{
    public bool IsPrivate { get; set; }
    public long ChatId { get; set; }
    public string Name { get; set; }
    public CreateChatEventUser[] ChatUsers { get; set; }
}

public class CreateChatEventUser
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}