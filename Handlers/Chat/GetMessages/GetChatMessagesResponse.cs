using WebApplication2.Handlers.Interfaces;

namespace WebApplication2.Handlers.Chat.GetMessages;

public class GetChatMessagesResponse : IPaginationResponse
{
    public ICollection<ChatMessageReponse> ChatMessages { get; set; }
    public long EntitiesLeft { get; set; }
}
