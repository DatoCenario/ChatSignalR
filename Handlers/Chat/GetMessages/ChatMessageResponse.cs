using System.Collections.Generic;

namespace WebApplication2.Handlers.Chat.GetMessages;

public class ChatMessageReponse
{
    public long Id { get; set; }
    public long ChatId { get; set; }
    public long OwnerId { get; set; }
    public string Text { get; set; }
    public string SendTime { get; set; }
    public string SenderName { get; set; }
    public bool IsMe { get; set; }
    public ICollection<ChatImageResponse> MessageImages { get; set; }
}