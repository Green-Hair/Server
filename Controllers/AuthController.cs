using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]/[action]")]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private IPasswordHasher<IdentityUser> _passwordHasher;
		private ILogger<AuthController> _logger;
		private IConfiguration _configuration;

		public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager
			, IPasswordHasher<IdentityUser> passwordHasher, ILogger<AuthController> logger, IConfiguration config)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_logger = logger;
			_passwordHasher = passwordHasher;
			_configuration = config;
		}

		/// <summary>
		/// 注册账户
		/// </summary>
		/// <param name="name">用户名</param>
		/// <param name="email">用户邮箱</param>
		/// <param name="password">密码 ( 最小长度为 6 )</param>
		/// <returns>是否成功</returns>
		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> Register([FromBody] RegisterVM model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}
			var user = new IdentityUser()
			{
				UserName = model.Name,
				Email = model.Email,
			};
			var result = await _userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
			{
				return Ok(new {
					status = 200,
					msg = "注册成功"
				});
			}
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("error", error.Description);
			}
			return BadRequest(new {
				status = 400,
				msg = new[] {
					result.Errors
				}
			});
		}

		/// <summary>
		/// 获取token
		/// </summary>
		/// <param name="email">用户邮箱</param>
		/// <param name="password">密码</param>
		/// <returns>带有 token 的 json</returns>
        [HttpPost]
        public async Task<IActionResult> Token([FromBody] LoginVM creds) {
            if (await CheckPassword(creds)) {
				var user = await _userManager.FindByEmailAsync(creds.Email);
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                byte[] secret = Encoding.ASCII.GetBytes(_configuration["jwtSecret"]);
                SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor {
                    Subject = new ClaimsIdentity(new Claim[] {
                        new Claim(ClaimTypes.Name, user.UserName)
                    }),
                    Expires = DateTime.UtcNow.AddHours(24),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(secret),
                            SecurityAlgorithms.HmacSha256Signature)
                };
                SecurityToken token = handler.CreateToken(descriptor);
                return Ok(new {
                    status = 200,
					msg = "获取 Token 成功",
                    token = handler.WriteToken(token)
                });
            }
            return Unauthorized();
        }

		private async Task<bool> CheckPassword(LoginVM creds) {
            var user = await _userManager.FindByEmailAsync(creds.Email);
            if (user != null) {
                return (await _signInManager.CheckPasswordSignInAsync(user,
                    creds.Password, true)).Succeeded;
            }
            return false;
        }
	}