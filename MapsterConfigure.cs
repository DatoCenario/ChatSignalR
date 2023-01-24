using System.Linq;
using Mapster;
using WebApplication2.Data.EF.Domain;
using WebApplication2.Handlers.Chat.GetChats;
using WebApplication2.Handlers.Chat.GetMessages;

namespace WebApplication2;

public class MapsterConfigure
{
    public static void Configure(TypeAdapterConfig config)
    {
        config.ForType<ChatUser, ChatUserResponse>()
            .Map(d => d.FirstName, s => s.User.FirstName)
            .Map(d => d.LastName, s => s.User.LastName);

        config.ForType<ChatMessage, ChatMessageReponse>()
            .Map(d => d.SenderName, m => $"{m.Owner.FirstName} {m.Owner.LastName}")
            .Map(d => d.SendTime, s => s.SendTime.ToShortTimeString())
            .Map(d => d.MessageImages, s => s.MessageImageLinks);

        config.ForType<ChatMessageImageLink, ChatImageResponse>()
            .Map(d => d.Base64Text, s => s.Image.Base64Text);
    }
}