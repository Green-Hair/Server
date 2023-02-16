
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    public int UserId {get;set;}
    public string UUID { get; set; }
    public string? UserName { get; set; }
    public string? Email {get;set;}
    public string? Password {get; set;}
}