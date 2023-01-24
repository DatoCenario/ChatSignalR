using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace WebApplication2.Data.EF.Domain;

public class ChatMessage: IEntity
{
    public long Id { get; set; }
    public long ChatId { get; set; }
    public long OwnerId { get; set; }
    public string Text { get; set; }
    public DateTime SendTime { get; set; }
    public virtual ICollection<ChatMessageImageLink> MessageImageLinks { get; set; }
    public virtual Chat Chat { get; set; }
    public virtual User Owner { get; set; }
}