using WebApplication2.Handlers.Interfaces;

namespace WebApplication2.Handlers.Chat.GetMessages;

public class GetChatMessagesRequest : CheckUserInChatQueryInBase<GetChatMessagesResponse>, IPaginationRequest
{

    public int Offset { get; set; }

    public int Count { get; set; }
}