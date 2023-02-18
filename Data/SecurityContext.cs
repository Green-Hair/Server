using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class SecurityContext : IdentityDbContext<IdentityUser>
{
    public SecurityContext(DbContextOptions<SecurityContext> options) : base(options)
    {
        
    }
}