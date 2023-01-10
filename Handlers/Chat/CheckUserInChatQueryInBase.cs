using MediatR;

namespace WebApplication2.Handlers.Chat;

public class CheckUserInChatQueryInBase<TOut>: IRequest<TOut> where TOut: class
{
    public long ChatId { get; set; }
}