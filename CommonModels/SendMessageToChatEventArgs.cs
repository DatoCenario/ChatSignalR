namespace WebApplication2.CommonModels;

public class SendMessageToChatEventArgs
{
    public bool IsMe { get; set; }
    public long ChatId { get; set; }
    public long MessageId { get; set; }
    public string SenderName { get; set; }
    public string Text { get; set; }
    public string SendTime { get; set; }
    public ICollection<SendMessageToChatEventImage> Images { get; set; }
}

public class SendMessageToChatEventImage
{
    public string Base64String { get; set; }
}