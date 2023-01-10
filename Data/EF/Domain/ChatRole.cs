namespace WebApplication2.Data.EF.Domain;

public class ChatRole: IEntity
{
    public bool IsAdmin { get; set; }
    public string Name { get; set; }
    public long Id { get; set; }
}