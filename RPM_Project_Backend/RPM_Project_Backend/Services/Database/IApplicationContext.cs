using Microsoft.EntityFrameworkCore;
using RPM_PR_LIB;
using Attribute = System.Attribute;

namespace RPM_Project_Backend.Services.Database;

public interface IApplicationContext : IDisposable
{
    DbSet<Image> Images { get; }

    DbSet<Address> Addresses { get; }

    DbSet<RPM_PR_LIB.Attribute> Attributes { get; }

    DbSet<BankCard> BankCards { get; }

    DbSet<CategoriesHaveAttribute> CategoriesHaveAttributes { get; }

    DbSet<Category> Categories { get; }

    DbSet<ListsHaveProduct> ListsHaveProducts { get; }

    DbSet<Order> Orders { get; }

    DbSet<OrdersHaveProduct> OrdersHaveProducts { get; }

    DbSet<Payment> Payments { get; }

    DbSet<Permission> Permissions { get; }

    DbSet<Product> Products { get; }

    DbSet<ProductList> ProductLists { get; }

    DbSet<ProductsHaveAttribute> ProductsHaveAttributes { get; }

    DbSet<Qiwi> Qiwis { get; }

    DbSet<Review> Reviews { get; }

    DbSet<Role> Roles { get; }

    DbSet<RoleHasPermission> RoleHasPermissions { get; }

    DbSet<Transaction> Transactions { get; }

    DbSet<User> Users { get; }
    
    int SaveChanges();
    Task<int> SaveChangesAsync();
    void MarkAsModified(Object item);    
}