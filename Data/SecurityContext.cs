using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class SecurityContext : IdentityDbContext<User>
{
    public SecurityContext(DbContextOptions<SecurityContext> options) : base(options)
    {
    }
}