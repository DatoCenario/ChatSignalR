using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using WebApplication2.Handlers.Chat.GetChats;
using WebApplication2.Handlers.Chat.GetMessages;
using WebApplication2.Handlers.Users;
using WebApplication2.Queries.Chat;

namespace WebApplication2.Queries.Users;

[ExtendObjectType("Query")]
public class UserQueries
{
    [Authorize]
    public class UserQueriesData
    {
        public async Task<GetUsersResponse> GetUsers(
            [Service] IMediator mediator,
            GetUsersQueryIn request) => await mediator.Send(request.Adapt<GetUsersRequest>());
    }

    public UserQueriesData Users => new UserQueriesData();
}

