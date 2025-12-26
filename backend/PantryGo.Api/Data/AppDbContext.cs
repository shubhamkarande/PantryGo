using Microsoft.EntityFrameworkCore;
using PantryGo.Api.Models.Entities;

namespace PantryGo.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.Property(e => e.Role)
                .HasConversion<string>()
                .HasMaxLength(20);
        });
        
        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Name);
        });
        
        // Address configuration
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
            
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Address)
                .WithMany(a => a.Orders)
                .HasForeignKey(e => e.AddressId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasOne(e => e.DeliveryPartner)
                .WithMany(u => u.DeliveryAssignments)
                .HasForeignKey(e => e.DeliveryPartnerId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // OrderItem configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Seed initial product data
        SeedProducts(modelBuilder);
    }
    
    private static void SeedProducts(ModelBuilder modelBuilder)
    {
        var products = new[]
        {
            // Fruits
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111101"), Name = "Fresh Apples", Description = "Crisp and sweet red apples", Price = 120.00m, Category = "Fruits", Stock = 100, Unit = "kg", ImageUrl = "/images/products/apples.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111102"), Name = "Bananas", Description = "Ripe yellow bananas", Price = 40.00m, Category = "Fruits", Stock = 150, Unit = "dozen", ImageUrl = "/images/products/bananas.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111103"), Name = "Oranges", Description = "Juicy oranges rich in Vitamin C", Price = 80.00m, Category = "Fruits", Stock = 80, Unit = "kg", ImageUrl = "/images/products/oranges.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111104"), Name = "Mangoes", Description = "Sweet Alphonso mangoes", Price = 200.00m, Category = "Fruits", Stock = 50, Unit = "kg", ImageUrl = "/images/products/mangoes.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111105"), Name = "Grapes", Description = "Fresh green seedless grapes", Price = 90.00m, Category = "Fruits", Stock = 60, Unit = "kg", ImageUrl = "/images/products/grapes.jpg" },
            
            // Vegetables
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111201"), Name = "Tomatoes", Description = "Fresh red tomatoes", Price = 30.00m, Category = "Vegetables", Stock = 200, Unit = "kg", ImageUrl = "/images/products/tomatoes.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111202"), Name = "Onions", Description = "Fresh onions", Price = 35.00m, Category = "Vegetables", Stock = 300, Unit = "kg", ImageUrl = "/images/products/onions.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111203"), Name = "Potatoes", Description = "Farm fresh potatoes", Price = 25.00m, Category = "Vegetables", Stock = 250, Unit = "kg", ImageUrl = "/images/products/potatoes.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111204"), Name = "Spinach", Description = "Fresh green spinach leaves", Price = 20.00m, Category = "Vegetables", Stock = 100, Unit = "bunch", ImageUrl = "/images/products/spinach.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111205"), Name = "Carrots", Description = "Crunchy orange carrots", Price = 45.00m, Category = "Vegetables", Stock = 120, Unit = "kg", ImageUrl = "/images/products/carrots.jpg" },
            
            // Dairy
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111301"), Name = "Milk", Description = "Fresh full cream milk", Price = 60.00m, Category = "Dairy", Stock = 200, Unit = "liter", ImageUrl = "/images/products/milk.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111302"), Name = "Butter", Description = "Creamy salted butter", Price = 55.00m, Category = "Dairy", Stock = 80, Unit = "200g", ImageUrl = "/images/products/butter.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111303"), Name = "Cheese", Description = "Processed cheese slices", Price = 120.00m, Category = "Dairy", Stock = 60, Unit = "200g", ImageUrl = "/images/products/cheese.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111304"), Name = "Yogurt", Description = "Fresh plain yogurt", Price = 45.00m, Category = "Dairy", Stock = 100, Unit = "400g", ImageUrl = "/images/products/yogurt.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111305"), Name = "Paneer", Description = "Fresh cottage cheese", Price = 90.00m, Category = "Dairy", Stock = 70, Unit = "200g", ImageUrl = "/images/products/paneer.jpg" },
            
            // Snacks
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111401"), Name = "Potato Chips", Description = "Crispy salted potato chips", Price = 30.00m, Category = "Snacks", Stock = 150, Unit = "pack", ImageUrl = "/images/products/chips.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111402"), Name = "Biscuits", Description = "Cream filled biscuits", Price = 25.00m, Category = "Snacks", Stock = 200, Unit = "pack", ImageUrl = "/images/products/biscuits.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111403"), Name = "Namkeen", Description = "Mixed crunchy namkeen", Price = 40.00m, Category = "Snacks", Stock = 100, Unit = "200g", ImageUrl = "/images/products/namkeen.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111404"), Name = "Chocolates", Description = "Milk chocolate bar", Price = 50.00m, Category = "Snacks", Stock = 120, Unit = "piece", ImageUrl = "/images/products/chocolate.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111405"), Name = "Nuts Mix", Description = "Roasted mixed nuts", Price = 150.00m, Category = "Snacks", Stock = 80, Unit = "200g", ImageUrl = "/images/products/nuts.jpg" },
            
            // Beverages
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111501"), Name = "Orange Juice", Description = "Fresh orange juice", Price = 80.00m, Category = "Beverages", Stock = 100, Unit = "liter", ImageUrl = "/images/products/orange-juice.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111502"), Name = "Cold Coffee", Description = "Ready to drink cold coffee", Price = 45.00m, Category = "Beverages", Stock = 120, Unit = "250ml", ImageUrl = "/images/products/cold-coffee.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111503"), Name = "Coconut Water", Description = "Natural coconut water", Price = 35.00m, Category = "Beverages", Stock = 150, Unit = "pack", ImageUrl = "/images/products/coconut-water.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111504"), Name = "Green Tea", Description = "Organic green tea bags", Price = 120.00m, Category = "Beverages", Stock = 80, Unit = "box", ImageUrl = "/images/products/green-tea.jpg" },
            new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111505"), Name = "Soda", Description = "Sparkling lemon soda", Price = 25.00m, Category = "Beverages", Stock = 200, Unit = "can", ImageUrl = "/images/products/soda.jpg" },
        };
        
        modelBuilder.Entity<Product>().HasData(products);
    }
}
