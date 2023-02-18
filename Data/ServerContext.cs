using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class ServerContext : DbContext
{
    public DbSet<IdentityUser> Users { get; set; }

    public ServerContext(DbContextOptions<ServerContext> options)
            : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUser>().ToTable("User");
        }
}
