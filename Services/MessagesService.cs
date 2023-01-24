using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using WebApplication2.CommonModels;
using WebApplication2.Data.EF;
using WebApplication2.Data.EF.Domain;
using WebApplication2.Hubs.Models.Chat;
using Image = WebApplication2.Data.EF.Domain.Image;

namespace WebApplication2.Services;

public class MessagesService
{
    private readonly int _maxImageBytes = 12000;
    private readonly Regex _replaceImageHeaderReg = new Regex(@"^data:image\/(png|jpg);base64,");

    private readonly UserManager<User> _userManager;
    private readonly ApplicationContext _applicationContext;
    private readonly HttpContextAccessor _contextAccessor;

    public MessagesService(UserManager<User> userManager, ApplicationContext applicationContext,
        HttpContextAccessor contextAccessor)
    {
        _userManager = userManager;
        _applicationContext = applicationContext;
        _contextAccessor = contextAccessor;
    }

    public async Task<SendMessageToChatEventArgs> SendMessageToChatAsync(
        User user,
        long chatId,
        string text = null,
        params string[] imagesBase64)
    {
        imagesBase64 ??= Array.Empty<string>();

        imagesBase64 = imagesBase64.Select(i =>
            {
                i = _replaceImageHeaderReg.Replace(i, "");
                var imageBytes = Convert.FromBase64String(i);
                if (imageBytes.Length > _maxImageBytes)
                {
                    // no need to load my server (not quite correct algorithm - optimizes size by delta but not bytes lenght)
                    using (var image = SixLabors.ImageSharp.Image.Load(imageBytes, new PngDecoder()))
                    {
                        var delta = Math.Sqrt(imageBytes.Length / _maxImageBytes);
                        image.Mutate(o => o.Resize(new Size
                        {
                            Width = (int)(image.Width / delta),
                            Height = (int)(image.Height / delta)
                        }));
                        i = image.ToBase64String(PngFormat.Instance);
                        i = _replaceImageHeaderReg.Replace(i, "");
                    }
                }

                return i;
            })
            .ToArray();

        var chat = await _applicationContext.Chats
            .Include(c => c.ChatUsers)
            .ThenInclude(x => x.User)
            .Where(c => c.Id == chatId)
            .FirstAsync();

        if (chat == null || (chat != null && chat.ChatUsers.All(u => u.UserId != user.Id)))
        {
            throw new Exception("Chat not found");
        }

        var mess = new ChatMessage
        {
            OwnerId = user.Id,
            Text = text,
            ChatId = chat.Id,
            SendTime = DateTime.Now,
            MessageImageLinks = imagesBase64.Select(i => new ChatMessageImageLink
                {
                    Image = new Image()
                    {
                        Base64Text = i
                    }
                })
                .ToArray()
        };
        await _applicationContext.ChatMessages.AddAsync(mess);

        await _applicationContext.SaveChangesAsync();

        return new SendMessageToChatEventArgs
        {
            MessageId = mess.Id,
            SenderName = $"{user.FirstName} {user.LastName}",
            Text = mess.Text,
            SendTime = mess.SendTime.ToShortTimeString(),
            ChatId = chatId,
            Images = imagesBase64.Select(x => new SendMessageToChatEventImage
            {
                Base64String = x
            }).ToArray()
        };
    }

    public Task<CreateChatEventArgs> CreateNewPrivateChat(User user, User other)
    {
        if (other == null)
        {
            throw new Exception();
        }

        return CreateNewChat(user, new List<User> { user, other }, null, true);
    }


    public async Task<CreateChatEventArgs> CreateNewChat(
        User creator,
        ICollection<User> users,
        string name,
        bool privateChat = false)
    {
        users = users.Union(new[] { creator }).ToArray();

        if (users == null || users.Any(u => false))
        {
            throw new Exception();
        }

        var chat = new Chat
        {
            ChatUsers = users.Select(u => new ChatUser
            {
                User = u,
                Inviter = creator
            }).ToArray(),
            Name = name,
            IsPrivate = privateChat
        };

        await _applicationContext.Chats.AddAsync(chat);

        await _applicationContext.SaveChangesAsync();

        return new CreateChatEventArgs
        {
            Name = chat.Name,
            ChatId = chat.Id,
            ChatUsers = chat.ChatUsers.Select(u => new CreateChatEventUser()
            {
                FirstName = u.User.FirstName,
                LastName = u.User.LastName,
                Id = u.UserId
            }).ToArray(),
            IsPrivate = privateChat
        };
    }

    public async Task<AddUserToChatEventArgs> AddUserToChat(User inviter, User user, long chatId)
    {
        var chat = await _applicationContext.Chats
            .Include(c => c.ChatUsers)
            .Where(c => c.Id == chatId)
            .FirstAsync();

        if (chat == null || (chat != null && chat.ChatUsers.All(u => u.UserId != user.Id)))
        {
            throw new Exception("Chat not found");
        }

        await _applicationContext.ChatUsers.AddAsync(new ChatUser
        {
            ChatId = chatId,
            InviterId = inviter.Id,
            UserId = user.Id
        });

        await _applicationContext.SaveChangesAsync();

        return new AddUserToChatEventArgs
        {
            ChatId = chatId,
            FirstName = user.FirstName
        };
    }

    public async Task<LeaveChatEventArgs> LeaveChat(User user, long chatId)
    {
        var chatUser = await _applicationContext.ChatUsers
            .Where(c => c.ChatId == chatId && c.UserId == user.Id)
            .FirstOrDefaultAsync();

        if (chatUser != null)
        {
            _applicationContext.ChatUsers.Remove(chatUser);
        }
        else
        {
            return null;
        }

        await _applicationContext.SaveChangesAsync();

        return new LeaveChatEventArgs
        {
            ChatId = chatId,
            LeavedUserName = $"{user.FirstName} {user.LastName}"
        };
    }
}