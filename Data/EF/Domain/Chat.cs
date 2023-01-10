using System.Collections.Generic;

namespace WebApplication2.Data.EF.Domain;

public class Chat: IEntity
{
    public long Id { get; set; }
    public string Name { get; set; }
    
    public virtual ICollection<ChatUser> ChatUsers { get; set; }
    public virtual ICollection<ChatMessage> Messages { get; set; }
}