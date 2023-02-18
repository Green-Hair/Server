using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
public class UserController : ControllerBase
{

    private readonly ILogger<UserController> _logger;
    private SecurityContext _context;

    public UserController(ILogger<UserController> logger, SecurityContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet(Name = "GetUser")]
    public async Task<IEnumerable<User>> GetUser()
    {
        return await _context.Users.ToArrayAsync();
    }
}
