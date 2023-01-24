using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate.AspNetCore.Authorization;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data.EF;
using WebApplication2.Data.EF.Domain;
using WebApplication2.Handlers.Chat.GetChats;
using WebApplication2.Handlers.Chat.GetMessages;
using WebApplication2.Handlers.Users;

namespace WebApplication2.Controllers;

// Add graphQL to front and remove it !!

[Route("api/{controller}")]
[Authorize]
public class ChatController: Controller
{
    private readonly IMediator _mediator;
    private readonly HttpContextAccessor _contextAccessor;
    private readonly ApplicationContext _applicationContext;
    private readonly UserManager<User> _userManager;

    public ChatController(IMediator mediator, UserManager<User> userManager, HttpContextAccessor contextAccessor, ApplicationContext applicationContext)
    {
        _mediator = mediator;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
        _applicationContext = applicationContext;
    }

    [HttpPost("GetChats")]
    public Task<ICollection<GetChatsResponse>> GetChats([FromBody] GetChatsRequest request)
    {
        return _mediator.Send(request);
    }
    
    [HttpPost("GetMessages")]
    public Task<GetChatMessagesResponse> GetMessages([FromBody] GetChatMessagesRequest request)
    {
        return _mediator.Send(request);
    }
    
    [HttpPost("GetUsers")]
    public Task<GetUsersResponse> GetUsers([FromBody] GetUsersRequest request)
    {
        return _mediator.Send(request);
    }
}