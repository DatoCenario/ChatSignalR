using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data.EF;

namespace WebApplication2.Handlers.Users;

public class GetUsersHandler: IRequestHandler<GetUsersRequest, GetUsersResponse>
{
    private readonly ApplicationContext _applicationContext;
    private readonly TypeAdapterConfig _typeAdapterConfig;
    private readonly HttpContextAccessor _contextAccessor;

    public GetUsersHandler(ApplicationContext applicationContext, HttpContextAccessor contextAccessor)
    {
        _applicationContext = applicationContext;
        _contextAccessor = contextAccessor;
    }

    public async Task<GetUsersResponse> Handle(GetUsersRequest request, CancellationToken cancellationToken)
    {
        var usersQuery = _applicationContext.Users.AsQueryable();

        if (request.IdFilter != null && request.IdFilter.Any())
        {
            usersQuery = usersQuery
                .Where(u => request.IdFilter.Contains(u.Id));
        }
        
        if (request.ChatIdFilter != null && request.ChatIdFilter.Any())
        {
            usersQuery = usersQuery
                .Where(u => u.ChatUsers.Any(cu => request.ChatIdFilter.Contains(cu.ChatId)));
        }
        
        if (!string.IsNullOrEmpty(request.FullNameFilter))
        {
            usersQuery = usersQuery
                .Where(u => (u.FirstName + " " + u.LastName).Contains(request.FullNameFilter) ||
                            (u.LastName + " " + u.FirstName).Contains(request.FullNameFilter));
        }
        
        if (!string.IsNullOrEmpty(request.PhoneOrMailFilter))
        {
            usersQuery = usersQuery
                .Where(u => EF.Functions.ILike(u.Email, $"%{request.PhoneOrMailFilter.Trim()}%") ||
                                                                          EF.Functions.ILike(u.PhoneNumber, $"%{request.PhoneOrMailFilter.Trim()}%"));
        }

        if (request.ExcludeMe == true)
        {
            usersQuery = usersQuery.Where(u => u.UserName != _contextAccessor.HttpContext.User.Identity.Name);
        }
        
        if (request.ExcludeHasChatsWithMe == true)
        {
            usersQuery = usersQuery
                .Where(u => u.ChatUsers.All(c => !c.Chat.IsPrivate || 
                                                 c.Chat.ChatUsers.All(cu => cu.User.UserName != _contextAccessor.HttpContext.User.Identity.Name)));
        }

        if (request.AdditionalIds != null && request.AdditionalIds.Any())
        {
            usersQuery = usersQuery
                .Union(_applicationContext.Users.Where(u => request.AdditionalIds.Contains(u.Id)));
        }

        var allCount = await usersQuery.CountAsync();
        
        var users = await usersQuery
            .Skip(request.Offset)
            .Take(request.Count)
            .ProjectToType<UserResponse>(_typeAdapterConfig)
            .ToArrayAsync();

        return new GetUsersResponse
        {
            EntitiesLeft = Math.Max(allCount - request.Offset - users.Length, 0),
            
            Users = users
        };
    }
}