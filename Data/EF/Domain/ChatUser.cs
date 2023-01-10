namespace WebApplication2.Data.EF.Domain;

public class ChatUser: IEntity
{
    public long Id { get; set; }
    
    public long ChatId { get; set; }
    public long UserId { get; set; }

    public virtual User User { get; set; }
    public virtual Chat Chat { get; set; }
}