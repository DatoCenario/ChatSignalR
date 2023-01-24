using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data.EF;
using WebApplication2.Data.EF.Domain;

namespace WebApplication2.Handlers.Chat.GetMessages;

public class GetChatMessagesHadnlder: CheckUserInChatQueryHandlerBase<GetChatMessagesRequest, GetChatMessagesResponse>
{
    private readonly TypeAdapterConfig _typeAdapterConfig;
    private readonly UserManager<User> _userManager;
    private readonly HttpContextAccessor _contextAccessor;
    
    public GetChatMessagesHadnlder(UserManager<User> userManager, HttpContextAccessor contextAccessor, ApplicationContext context, TypeAdapterConfig typeAdapterConfig) : base(userManager, contextAccessor, context)
    {
        _userManager = userManager;
        _contextAccessor = contextAccessor;
        _typeAdapterConfig = typeAdapterConfig;
    }

    protected override async Task<GetChatMessagesResponse> InnerHandle(GetChatMessagesRequest request, CancellationToken cancellationToken)
    {
        request.Count = request.Count <= 0 ? int.MaxValue : request.Count;
        
        var me = await _userManager.FindByNameAsync(_contextAccessor.HttpContext.User.Identity.Name);
        
        var allCount = await Context.ChatMessages
            .Where(c => c.ChatId == request.ChatId)
            .CountAsync();

        var messages = await Context.ChatMessages
            .Include(x => x.Owner)
            .Include(m => m.MessageImageLinks)
            .ThenInclude(i => i.Image)
            .Where(c => c.ChatId == request.ChatId)
            .Skip(request.Offset)
            .Take(request.Count)
            .ToArrayAsync();

        var messagesData = messages
            .Select(m => m.Adapt<ChatMessageReponse>(_typeAdapterConfig))
            .ToArray();

        foreach (var mess in messagesData)
        {
            if (mess.OwnerId == me.Id)
            {
                mess.IsMe = true;
            }
        }
        
        return new GetChatMessagesResponse
        {
            ChatMessages = messagesData,

            EntitiesLeft = Math.Max(0, allCount - request.Offset - request.Count)
        };
    }
}