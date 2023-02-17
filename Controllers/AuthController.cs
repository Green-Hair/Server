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
[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly RoleManager<Role> _roleManager;
		private IPasswordHasher<User> _passwordHasher;
		private ILogger<AuthController> _logger;
		private IConfiguration _configuration;

		public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager
			, IPasswordHasher<User> passwordHasher, ILogger<AuthController> logger, IConfiguration config)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_logger = logger;
			_passwordHasher = passwordHasher;
			_configuration = config;
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register([FromBody] RegisterVM model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}
			var user = new User()
			{
				UserName = model.Name,
				Email = model.Email,
			};
			var result = await _userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
			{
				return Ok(result);
			}
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("error", error.Description);
			}
			return BadRequest(result.Errors);
		}

        [HttpPost("token")]
        public async Task<IActionResult> Token([FromBody] LoginVM creds) {
            if (await CheckPassword(creds)) {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                byte[] secret = Encoding.ASCII.GetBytes(_configuration["jwtSecret"]);
                SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor {
                    Subject = new ClaimsIdentity(new Claim[] {
                        new Claim(ClaimTypes.Email, creds.Email)
                    }),
                    Expires = DateTime.UtcNow.AddHours(24),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(secret),
                            SecurityAlgorithms.HmacSha256Signature)
                };
                SecurityToken token = handler.CreateToken(descriptor);
                return Ok(new {
                    success = true,
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