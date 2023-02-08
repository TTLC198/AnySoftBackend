using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RPM_PR_LIB;
using Attribute = RPM_PR_LIB.Attribute;

namespace RPM_Project_Backend.Services.Database;

public partial class ApplicationContext : DbContext
{
    public ApplicationContext()
    {
    }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Attribute> Attributes { get; set; }

    public virtual DbSet<BankCard> BankCards { get; set; }

    public virtual DbSet<CategoriesHaveAttribute> CategoriesHaveAttributes { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ListsHaveProduct> ListsHaveProducts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrdersHaveProduct> OrdersHaveProducts { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductList> ProductLists { get; set; }

    public virtual DbSet<ProductsHaveAttribute> ProductsHaveAttributes { get; set; }

    public virtual DbSet<Qiwi> Qiwis { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleHasPermission> RoleHasPermissions { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DEVICE,1433;Database=rmp_project_new;Integrated Security=SSPI;TrustServerCertificate=True");
}
