namespace WebApplication2.Hubs.Models.Chat;

public class SendMessageToChatRequest
{
    public long ChatId { get; set; }

    public string Text { get; set; }
    public string[] ImagesBase64 { get; set; }
}