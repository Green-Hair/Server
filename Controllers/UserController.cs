using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    private readonly ILogger<UserController> _logger;
    private ServerContext _context;

    public UserController(ILogger<UserController> logger, ServerContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet(Name = "GetUsers")]
    public async Task<IEnumerable<User>> Get()
    {
        return await _context.Users.ToArrayAsync();
    }
}
