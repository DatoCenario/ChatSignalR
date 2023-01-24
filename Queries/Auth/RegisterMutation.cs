using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebApplication2.CommonModels;
using WebApplication2.Data.EF.Domain;
using WebApplication2.Handlers.Auth;
using WebApplication2.Services;

namespace WebApplication2.Queries.Auth;

[ExtendObjectType("Mutation")]
public class RegisterMutations
{
    [Authorize]
    public class RegisterMutationsData
    {
        public async Task<RegisterResponse> Register(
            [Service] IMediator mediator,
            RegisterQueryIn request) => await mediator.Send(request.Adapt<RegisterRequest>());
    }

    public RegisterMutationsData Register => new RegisterMutationsData();
}

