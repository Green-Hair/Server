using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

    /// <summary>
    /// 获取当前用户的信息
    /// </summary>
    /// <returns>当前用户的信息</returns>
    [HttpGet]
    public async Task<IActionResult> GetUserInfomation()
    {
        var user = from u in _context.Users 
                    where u.UserName == HttpContext.User.Identity.Name 
                    select u;

        var result = await user.FirstOrDefaultAsync();

			if (result != null)
			{
				return Ok(new {
                    status = 200,
                    msg = "获取用户信息成功",
                    result = result
                });
			}
            else {
                return NotFound(new {
                    status = 404,
                    msg = "未找到用户信息"
                });
            }
    }
}
