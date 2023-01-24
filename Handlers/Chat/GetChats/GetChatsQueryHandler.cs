using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data.EF;
using WebApplication2.Data.EF.Domain;

namespace WebApplication2.Handlers.Chat.GetChats;

public class GetChatsQueryHandler: IRequestHandler<GetChatsRequest, ICollection<GetChatsResponse>>
{
    private readonly ApplicationContext _applicationContext;
    private readonly UserManager<User> _userManager;
    private readonly HttpContextAccessor _contextAccessor;
    private readonly TypeAdapterConfig _typeAdapterConfig;

    public GetChatsQueryHandler(ApplicationContext applicationContext, UserManager<User> userManager, HttpContextAccessor contextAccessor, TypeAdapterConfig typeAdapterConfig)
    {
        _applicationContext = applicationContext;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
        _typeAdapterConfig = typeAdapterConfig;
    }

    public async Task<ICollection<GetChatsResponse>> Handle(GetChatsRequest request, CancellationToken cancellationToken)
    {
        var me = await _userManager.FindByNameAsync(_contextAccessor.HttpContext.User.Identity.Name);
        
        var chatsQuery = _applicationContext.Chats.AsQueryable();

        if (!string.IsNullOrEmpty(request.NameFilter))
        {
            chatsQuery = chatsQuery
                .Where(c => EF.Functions.ILike(c.Name, $"%{request.NameFilter.Trim()}%"));
        }

        if (request.OnlyUserChats == true)
        {
            chatsQuery = chatsQuery
                .Where(c => c.ChatUsers.Any(u => u.UserId == me.Id));
        }

        var chats = await chatsQuery
            .Include(c => c.Messages)
            .ThenInclude(x => x.Owner)
            .Include(c => c.ChatUsers)
            .ThenInclude(u => u.User)
            .Where(c => c.ChatUsers.Any(u => u.UserId == me.Id) || c.IsPrivate == false)
            .ProjectToType<GetChatsResponse>(_typeAdapterConfig)
            .ToArrayAsync();

        var chatIds = chats.Select(x => x.Id).ToArray();
        var lastMessages = await _applicationContext.ChatMessages
            .Include(x => x.Owner)
            .Where(x => chatIds.Contains(x.ChatId))
            .GroupBy(x => x.ChatId)
            .Select(x => x.OrderByDescending(x => x.SendTime).First())
            .ToDictionaryAsync(x => x.ChatId, v => v);
        
        foreach (var chat in chats)
        {
            if (string.IsNullOrEmpty(chat.Name))
            {
                var other = chat.ChatUsers.FirstOrDefault(c => c.UserId != me.Id);
                if (other != null)
                {
                    chat.Name = $"{other.FirstName} {other.LastName}";
                }
                else
                {
                    chat.Name = $"{me.FirstName} {me.LastName}";
                }
            }
            
            var lastMess = lastMessages.GetValueOrDefault(chat.Id);
            if (lastMess != null)
            {
                chat.LastMessage = lastMess.Text;
                chat.LastMessageSender = $"{lastMess.Owner.FirstName} {lastMess.Owner.LastName}";
                chat.LastMessageSendTime = lastMess.SendTime.ToShortDateString();
            }
        }

        return chats;
    }
}