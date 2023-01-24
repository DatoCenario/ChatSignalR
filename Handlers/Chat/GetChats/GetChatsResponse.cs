using System.Collections.Generic;

namespace WebApplication2.Handlers.Chat.GetChats;

public class GetChatsResponse
{
    public bool IsPrivate { get; set; }
    public string? Name { get; set; }
    public string? LastMessage { get; set; }
    public string? LastMessageSender { get; set; }
    public string? LastMessageSendTime { get; set; }
    public long Id { get; set; }
    public ICollection<ChatUserResponse> ChatUsers { get; set; }
}

public class ChatUserResponse
{
    public long UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}