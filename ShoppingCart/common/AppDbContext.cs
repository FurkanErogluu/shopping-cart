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
        new Product { Id = 4, Name = "Simit", Price = 12.50m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 5, Name = "Yumurta (10'lu)", Price = 79.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 6, Name = "Beyaz Peynir 500g", Price = 159.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 7, Name = "Kaşar Peynir 400g", Price = 149.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 8, Name = "Tereyağı 250g", Price = 119.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 9, Name = "Yoğurt 1kg", Price = 69.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },

        // Fruits (kg)
        new Product { Id = 10, Name = "Elma", Price = 39.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Weight, DefaultUnitName = "Kg" },
        new Product { Id = 11, Name = "Muz", Price = 59.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Weight, DefaultUnitName = "Kg" },
        new Product { Id = 12, Name = "Portakal", Price = 34.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Weight, DefaultUnitName = "Kg" },
        new Product { Id = 13, Name = "Mandalina", Price = 44.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Weight, DefaultUnitName = "Kg" },
        new Product { Id = 14, Name = "Üzüm", Price = 89.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Weight, DefaultUnitName = "Kg" },
        new Product { Id = 15, Name = "Çilek", Price = 129.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Weight, DefaultUnitName = "Kg" },

        // Vegetables (kg)
        new Product { Id = 16, Name = "Salatalık", Price = 34.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Weight, DefaultUnitName = "Kg" },
        new Product { Id = 17, Name = "Patates", Price = 24.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Weight, DefaultUnitName = "Kg" },
        new Product { Id = 18, Name = "Soğan", Price = 19.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Weight, DefaultUnitName = "Kg" },
        new Product { Id = 19, Name = "Biber (Sivri)", Price = 69.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Weight, DefaultUnitName = "Kg" },
        new Product { Id = 20, Name = "Biber (Kapya)", Price = 74.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Weight, DefaultUnitName = "Kg" },
        new Product { Id = 21, Name = "Marul", Price = 29.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 22, Name = "Maydanoz", Price = 14.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 23, Name = "Limon", Price = 49.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Weight, DefaultUnitName = "Kg" },

        // Pantry
        new Product { Id = 24, Name = "Pirinç 1kg", Price = 79.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 25, Name = "Bulgur 1kg", Price = 54.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 26, Name = "Makarna", Price = 24.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 27, Name = "Salça 830g", Price = 89.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 28, Name = "Zeytinyağı 1L", Price = 219.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 29, Name = "Ayçiçek Yağı 1L", Price = 119.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 30, Name = "Tuz 750g", Price = 19.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 31, Name = "Şeker 1kg", Price = 54.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 32, Name = "Un 1kg", Price = 44.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },

        // Drinks
        new Product { Id = 33, Name = "Su 1.5L", Price = 12.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 34, Name = "Maden Suyu", Price = 9.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 35, Name = "Kola 1L", Price = 49.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 36, Name = "Meyve Suyu 1L", Price = 59.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },

        // Cleaning
        new Product { Id = 37, Name = "Bulaşık Deterjanı", Price = 89.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 38, Name = "Çamaşır Deterjanı", Price = 249.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 39, Name = "Kağıt Havlu", Price = 99.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" },
        new Product { Id = 40, Name = "Tuvalet Kağıdı", Price = 129.90m, DefaultUnit = ShoppingCart.Enums.UnitType.Piece, DefaultUnitName = "Adet" }
    );
    }
}