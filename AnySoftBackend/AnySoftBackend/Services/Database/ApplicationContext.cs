using Microsoft.EntityFrameworkCore;
using AnySoftBackend.Domain;

namespace AnySoftBackend.Services.Database;

public sealed class ApplicationContext : DbContext
{
    public ApplicationContext()
    {
        Database.EnsureCreated();
    }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(o => o.Orders)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder
            .Entity<Review>()
            .HasOne(o => o.User)
            .WithMany(o => o.Reviews)
            .OnDelete(DeleteBehavior.ClientCascade);
        
        modelBuilder
            .Entity<UsersHaveProducts>()
            .HasOne(o => o.User)
            .WithMany(o => o.UsersHaveProducts)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<OrdersHaveProduct>()
            .HasOne(o => o.Order)
            .WithMany(o => o.OrdersHaveProducts)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder
            .Entity<Transaction>()
            .HasOne(o => o.Payment)
            .WithMany(o => o.Transactions)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder
            .Entity<Image>()
            .HasOne(o => o.Product)
            .WithMany(o => o.Images)
            .OnDelete(DeleteBehavior.ClientCascade);
        
        modelBuilder
            .Entity<Product>()
            .HasMany(o => o.Images)
            .WithOne(o => o.Product)
            .OnDelete(DeleteBehavior.ClientCascade);
    }

    public DbSet<Image> Images { get; set; }

    public DbSet<Property> Properties { get; set; }

    public DbSet<Genre> Genres { get; set; }
    
    public DbSet<UsersHaveProducts> UsersHaveProducts { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrdersHaveProduct> OrdersHaveProducts { get; set; }

    public DbSet<Payment> Payments { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<ProductsHaveProperties> ProductsHaveProperties { get; set; }
    
    public DbSet<ProductsHaveGenres> ProductsHaveGenres { get; set; }

    public DbSet<Review> Reviews { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<User> Users { get; set; }
}
