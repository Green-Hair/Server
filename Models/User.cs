
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
		public DateTime JoinDate { get; set; }
		public DateTime JobTitle { get; set; }
		public string? Contract { get; set; }
}