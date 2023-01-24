using WebApplication2.Handlers.Interfaces;

namespace WebApplication2.Queries.Chat;

public class GetChatMessagesQueryIn: IPaginationRequest
{
    public int Offset { get; set; }
    public int Count { get; set; }
    public long ChatId { get; set; }
}