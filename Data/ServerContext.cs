using Microsoft.EntityFrameworkCore;

public class ServerContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public ServerContext(DbContextOptions<ServerContext> options)
            : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
        }
}
