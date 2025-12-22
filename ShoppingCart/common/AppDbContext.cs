using Microsoft.EntityFrameworkCore;
using ShoppingCart.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<UserConnection> UserConnections { get; set; } = null!;

    public DbSet<Product> Products { get; set; } = null!;

    public DbSet<ShoppingList> ShoppingLists { get; set; } = null!;
    public DbSet<ShoppingListMember> ShoppingListMembers { get; set; } = null!;
    public DbSet<ShoppingListProduct> ShoppingListProducts { get; set; } = null!;

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

        modelBuilder.Entity<ShoppingList>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IsCompleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<ShoppingListMember>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ShoppingListId });

            entity.HasOne(e => e.User)
                .WithMany(u => u.ShoppingListMemberships)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ShoppingList)
                .WithMany(l => l.Members)
                .HasForeignKey(e => e.ShoppingListId)
                .OnDelete(DeleteBehavior.Cascade);
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

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            

            entity.Property(e => e.Price)
                .IsRequired()
                .HasPrecision(18, 2); 
                
            entity.Property(e => e.DefaultUnit).IsRequired();
            entity.Property(e => e.DefaultUnitName).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<ShoppingListProduct>(entity =>
        {
            entity.HasKey(e => new { e.ShoppingListId, e.ProductId });

            entity.Property(e => e.Quantity).IsRequired()
                .HasPrecision(18, 2)
                .HasDefaultValue(1m); 
                
            entity.Property(e => e.IsChecked).HasDefaultValue(false);

            entity.HasOne(e => e.ShoppingList)
                .WithMany(l => l.Items)
                .HasForeignKey(e => e.ShoppingListId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Product>().HasData(
        new Product 
        { 
            Id = 1,
            Name = "Ekmek", 
            Price = 15.00m, 
            DefaultUnit = ShoppingCart.Enums.UnitType.Piece, 
            DefaultUnitName = "Adet" 
        },
        new Product 
        { 
            Id = 2, 
            Name = "Domates", 
            Price = 49.90m, 
            DefaultUnit = ShoppingCart.Enums.UnitType.Weight, 
            DefaultUnitName = "Kg" 
        },
        new Product 
        { 
            Id = 3, 
            Name = "1Lt Süt", 
            Price = 32.50m, 
            DefaultUnit = ShoppingCart.Enums.UnitType.Piece, 
            DefaultUnitName = "Adet" 
        }
    );
    }
}