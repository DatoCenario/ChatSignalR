using MediatR;
using WebApplication2.Handlers.Chat.GetMessages;

namespace WebApplication2.Queries.Chat;

[ExtendObjectType("Query")]
public class ChatQueries
{
    public class ChatQueriesData
    {
        public async Task<GetChatMessagesResponse> GetChatMessages(
            [Service] IMediator mediator,
            GetChatMessagesRequest request) => await mediator.Send(request);
    }

    public ChatQueriesData ChatMessages => new ChatQueriesData();
}

