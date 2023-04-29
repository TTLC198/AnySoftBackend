using Microsoft.EntityFrameworkCore;
using RPM_Project_Backend.Domain;

namespace RPM_Project_Backend.Services.Database;

public class ApplicationContext : DbContext
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
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder
            .Entity<CartsHaveProduct>()
            .HasOne(o => o.ShoppingCart)
            .WithMany(o => o.CartsHaveProducts)
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
    }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    public virtual DbSet<BankCard> BankCards { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }
    
    public virtual DbSet<CartsHaveProduct> CartsHaveProduct { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrdersHaveProduct> OrdersHaveProducts { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

    public virtual DbSet<ProductsHaveProperties> ProductsHaveProperties { get; set; }
    
    public virtual DbSet<ProductsHaveGenres> ProductsHaveGenres { get; set; }

    public virtual DbSet<Qiwi> Qiwis { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }
}
