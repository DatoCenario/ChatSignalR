namespace WebApplication2.Data.EF.Domain;

public class ChatMessageImageLink: IEntity
{
    public long Id { get; set; }
    
    public long ChatMessageId { get; set; }
    public long ImageId { get; set; }
    public virtual ChatMessage ChatMessage { get; set; }
    public virtual  Image Image { get; set; }
}