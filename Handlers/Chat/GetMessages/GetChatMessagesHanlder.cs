using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data.EF;
using WebApplication2.Data.EF.Domain;

namespace WebApplication2.Handlers.Chat.GetMessages;

public class ChatMessageReponse
{
    public string Text { get; set; }
}

public class GetChatMessagesHanlder: CheckUserInChatQueryHandlerBase<GetChatMessagesRequest, GetChatMessagesResponse>
{
    public GetChatMessagesHanlder(UserManager<User> userManager, HttpContextAccessor contextAccessor, ApplicationContext context) : base(userManager, contextAccessor, context)
    {
    }

    protected override async Task<GetChatMessagesResponse> InnerHandle(GetChatMessagesRequest request, CancellationToken cancellationToken)
    {
        var allCount = await Context.ChatMessages
            .Where(c => c.ChatId == request.ChatId)
            .CountAsync();

        var messages = await Context.ChatMessages
            .Where(c => c.ChatId == request.ChatId)
            .Skip(request.Offset)
            .Take(request.Count)
            .ToArrayAsync();

        return new GetChatMessagesResponse
        {
            ChatMessages = messages.Select(m => new ChatMessageReponse
                {
                    Text = m.Text
                })
                .ToArray(),

            EntitiesLeft = Math.Max(0, allCount - request.Offset - request.Count)
        };
    }
}