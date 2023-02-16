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

		public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager
			, IPasswordHasher<User> passwordHasher, ILogger<AuthController> logger)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_logger = logger;
			_passwordHasher = passwordHasher;
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
				UserName = model.Email,
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

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login([FromBody] LoginVM model)
		{
			try
			{
				var user = await _userManager.FindByNameAsync(model.Email);
				if (user == null)
				{
					return Unauthorized();
				}
				if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Success)
				{
					var userClaims = await _userManager.GetClaimsAsync(user);

					var claims = new[]
					{
						  new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
						  new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
						  new Claim(JwtRegisteredClaimNames.Email, user.Email)
						}.Union(userClaims);

					var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YSTC114514^&%&^%&^1919810%%%"));
					var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

					var jwtSecurityToken = new JwtSecurityToken(
					  claims: claims,
					  expires: DateTime.UtcNow.AddMinutes(60),
					  signingCredentials: signingCredentials
					  );
					return Ok(new
					{
						token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
						expiration = jwtSecurityToken.ValidTo
					});
				}
				return Unauthorized();
			}
			catch (Exception ex)
			{
				_logger.LogError($"error while creating token: {ex}");
				return StatusCode((int)HttpStatusCode.InternalServerError, "error while creating token");
			}
		}
	}