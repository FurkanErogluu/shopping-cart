using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<UserConnection> UserConnections { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FollowId).IsRequired().HasMaxLength(8);
            
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.FollowId).IsUnique();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
            
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.UserId);
            
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserConnection>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => new { e.User1Id, e.User2Id }).IsUnique();
            
            entity.HasOne(e => e.User1)
                .WithMany(u => u.ConnectionsInitiated)
                .HasForeignKey(e => e.User1Id)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.User2)
                .WithMany(u => u.ConnectionsReceived)
                .HasForeignKey(e => e.User2Id)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}