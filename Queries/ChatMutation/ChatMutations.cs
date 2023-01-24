using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using WebApplication2.CommonModels;
using WebApplication2.Data.EF.Domain;
using WebApplication2.Handlers.Chat.GetMessages;
using WebApplication2.Services;

namespace WebApplication2.Queries.ChatMutation;

[ExtendObjectType("Mutation")]
public class ChatMutations
{
    [Authorize]
    public class ChatMutationsData
    {
        public async Task<CreateChatEventArgs> CreateNewChatWithOneUser(
            [Service] HttpContextAccessor contextAccessor,
            [Service] UserManager<User> userManager,
            [Service] MessagesService messagesService,
            string name)
        {
            var user = await userManager.FindByNameAsync(contextAccessor.HttpContext.User.Identity.Name);

            return await messagesService.CreateNewChat(user, new [] {user}, name);
        }
        
        public async Task<CreateChatEventArgs> CreatePrivateChat(
            [Service] HttpContextAccessor contextAccessor,
            [Service] UserManager<User> userManager,
            [Service] MessagesService messagesService,
            long otherUserId)
        {
            var user = await userManager.FindByNameAsync(contextAccessor.HttpContext.User.Identity.Name);
            var other = await userManager.FindByIdAsync(otherUserId.ToString());

            return await messagesService.CreateNewPrivateChat(user, other);
        }
        
        public async Task<SendMessageToChatEventArgs> SendMessageToChat(
            [Service] HttpContextAccessor contextAccessor,
            [Service] UserManager<User> userManager,
            [Service] MessagesService messagesService,
            long chatId,
            string text)
        {
            var user = await userManager.FindByNameAsync(contextAccessor.HttpContext.User.Identity.Name);

            return await messagesService.SendMessageToChatAsync(user, chatId, text);
        }
        
        public async Task AddUserToChat(
            [Service] HttpContextAccessor contextAccessor,
            [Service] UserManager<User> userManager,
            [Service] MessagesService messagesService,
            long chatId,
            long userId)
        {
            var inviter = await userManager.FindByNameAsync(contextAccessor.HttpContext.User.Identity.Name);
            var user = await userManager.FindByIdAsync(userId.ToString());

            await messagesService.AddUserToChat(inviter, user, chatId);
        }
    }

    public ChatMutationsData Chat => new ChatMutationsData();
}

