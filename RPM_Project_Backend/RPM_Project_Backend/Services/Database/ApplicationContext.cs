using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RPM_PR_LIB;

namespace RPM_Project_Backend.Services.Database;

public sealed class ApplicationContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Moderator> Moderators { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<ProductOrder> ProductOrders { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
    {
        Database.EnsureCreated();
    }
}