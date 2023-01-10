using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data.EF;
using WebApplication2.Data.EF.Domain;

namespace WebApplication2.Controllers;

[Route("{controller}")]
[Authorize]
public class ChatController: Controller
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationContext _context;

    public ChatController(UserManager<User> userManager, ApplicationContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

        var chats = await _context.Chats
            .Include(c => c.Messages)
            .ThenInclude(m => m.Owner)
            .Where(c => c.ChatUsers.Any(u => u.Id == user.Id))
            .ToArrayAsync();

        var model = new MessengerViewModel
        {
            Chats = chats.Select(c => new ChatViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Messages = c.Messages.Select(m => new ChatMessagesModel
                {
                    Id = m.Id,
                    Text = m.Text,
                    Time = DateTime.Now,
                    Username = m.Owner.FirstName
                }).ToArray()
            }).ToArray()
        };
        
        return View(model);
    }
}

public class MessengerViewModel
{
    public ICollection<ChatViewModel> Chats { get; set; }
}

public class ChatViewModel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public IList<ChatMessagesModel> Messages { get; set; }
}

public class ChatMessagesModel
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string Text { get; set; }
    public DateTime Time { get; set; }
}
