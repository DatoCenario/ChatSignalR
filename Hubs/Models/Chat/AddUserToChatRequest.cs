using Newtonsoft.Json;

namespace WebApplication2.Hubs.Models.Chat;

public class AddUserToChatRequest
{
    [JsonProperty("chatId")]
    public long ChatId { get; set; }
    
    [JsonProperty("userId")]
    public long UserId { get; set; }
}