using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Data.EF.Domain;

public class Image: IEntity
{
    public long Id { get; set; }
    
    public string Base64Text { get; set; }
    public virtual ICollection<ChatMessageImageLink> MessageImageLinks { get; set; }
}