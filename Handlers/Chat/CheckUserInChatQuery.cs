using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data.EF;
using WebApplication2.Data.EF.Domain;

namespace WebApplication2.Handlers.Chat;

public abstract class CheckUserInChatQueryHandlerBase<TRequest, TOut>: IRequestHandler<TRequest, TOut>
    where TRequest: CheckUserInChatQueryInBase<TOut>
    where TOut: class
{
    protected readonly UserManager<User> UserManager;
    protected readonly HttpContextAccessor ContextAccessor;
    protected readonly ApplicationContext Context;

    public CheckUserInChatQueryHandlerBase(UserManager<User> userManager, HttpContextAccessor contextAccessor, ApplicationContext context)
    {
        UserManager = userManager;
        ContextAccessor = contextAccessor;
        Context = context;
    }
    

    public async Task<TOut> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var user = await UserManager.FindByNameAsync(ContextAccessor.HttpContext?.User.Identity?.Name);

        if (user == null)
        {
            throw new Exception();
        }

        var anyUserChat = await Context.Users
            .Where(u => u.Id == user.Id)
            .SelectMany(u => u.ChatUsers)
            .Select(c => c.ChatId)
            .AnyAsync(c => c == request.ChatId);

        if (!anyUserChat)
        {
            throw new Exception();
        }

        return await InnerHandle(request, cancellationToken);
    }
    
    protected abstract Task<TOut> InnerHandle(TRequest request, CancellationToken cancellationToken);
}