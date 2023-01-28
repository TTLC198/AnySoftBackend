using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RPM_PR_LIB;
using Attribute = RPM_PR_LIB.Attribute;

namespace RPM_Project_Backend.Services.Database;

public partial class RmpProjectNewContext : DbContext
{
    public RmpProjectNewContext()
    {
    }

    public RmpProjectNewContext(DbContextOptions<RmpProjectNewContext> options)
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

    public virtual DbSet<Seller> Sellers { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DEVICE,1433;Database=rmp_project_new;Integrated Security=SSPI;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AdId).HasName("Addresses_pk");

            entity.Property(e => e.AdId).HasColumnName("ad_id");
            entity.Property(e => e.AdStreet)
                .HasMaxLength(256)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ad_street");
            entity.Property(e => e.AdUId).HasColumnName("ad_u_id");
            entity.Property(e => e.City)
                .HasMaxLength(256)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(256)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("country");
            entity.Property(e => e.State)
                .HasMaxLength(256)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("state");
        });

        modelBuilder.Entity<Attribute>(entity =>
        {
            entity.HasKey(e => e.AtrId).HasName("Attributes_pk");

            entity.Property(e => e.AtrId).HasColumnName("atr_id");
            entity.Property(e => e.AtrName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("atr_name");
        });

        modelBuilder.Entity<BankCard>(entity =>
        {
            entity.HasKey(e => e.BcId).HasName("Bank_cards_pk");

            entity.ToTable("Bank_cards");

            entity.Property(e => e.BcId).HasColumnName("bc_id");
            entity.Property(e => e.BcCvc).HasColumnName("bc_cvc");
            entity.Property(e => e.BcExpirationDate)
                .HasColumnType("date")
                .HasColumnName("bc_expiration_date");
            entity.Property(e => e.BcName)
                .HasMaxLength(256)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("bc_name");
            entity.Property(e => e.BcNumber).HasColumnName("bc_number");
            entity.Property(e => e.BcPaymentId).HasColumnName("bc_payment_id");

            entity.HasOne(d => d.BcPayment).WithMany(p => p.BankCards)
                .HasForeignKey(d => d.BcPaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Bank_cards_Payments_pay_id_fk");
        });

        modelBuilder.Entity<CategoriesHaveAttribute>(entity =>
        {
            entity.HasKey(e => e.ChaId).HasName("Categories_have_attributes_pk");

            entity.ToTable("Categories_have_attributes");

            entity.Property(e => e.ChaId).HasColumnName("cha_id");
            entity.Property(e => e.ChaAtrId).HasColumnName("cha_atr_id");
            entity.Property(e => e.ChaCatId).HasColumnName("cha_cat_id");

            entity.HasOne(d => d.ChaAtr).WithMany(p => p.CategoriesHaveAttributes)
                .HasForeignKey(d => d.ChaAtrId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Categories_have_attributes_Attributes_atr_id_fk");

            entity.HasOne(d => d.ChaCat).WithMany(p => p.CategoriesHaveAttributes)
                .HasForeignKey(d => d.ChaCatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Categories_have_attributes_Categories_cat_id_fk");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CatId).HasName("Categories_pk");

            entity.Property(e => e.CatId).HasColumnName("cat_id");
            entity.Property(e => e.CatName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("cat_name");
            entity.Property(e => e.CatParentId).HasColumnName("cat_parent_id");

            entity.HasOne(d => d.CatParent).WithMany(p => p.InverseCatParent)
                .HasForeignKey(d => d.CatParentId)
                .HasConstraintName("Categories_Categories_cat_id_fk");
        });

        modelBuilder.Entity<ListsHaveProduct>(entity =>
        {
            entity.HasKey(e => e.LhpId).HasName("Lists_have_products_pk");

            entity.ToTable("Lists_have_products");

            entity.Property(e => e.LhpId).HasColumnName("Lhp_id");
            entity.Property(e => e.LhpPlId).HasColumnName("lhp_pl_id");
            entity.Property(e => e.LhpProId).HasColumnName("lhp_pro_id");

            entity.HasOne(d => d.LhpPl).WithMany(p => p.ListsHaveProducts)
                .HasForeignKey(d => d.LhpPlId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Lists_have_products_Product_lists_pl_id_fk");

            entity.HasOne(d => d.LhpPro).WithMany(p => p.ListsHaveProducts)
                .HasForeignKey(d => d.LhpProId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Lists_have_products_Products_pro_id_fk");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrId).HasName("Orders_pk");

            entity.Property(e => e.OrId).HasColumnName("or_id");
            entity.Property(e => e.OrAdId).HasColumnName("or_ad_id");
            entity.Property(e => e.OrFcost).HasColumnName("or_fcost");
            entity.Property(e => e.OrNumber).HasColumnName("or_number");
            entity.Property(e => e.OrSId).HasColumnName("or_s_id");
            entity.Property(e => e.OrStatus)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("or_status");
            entity.Property(e => e.OrTime)
                .HasColumnType("datetime")
                .HasColumnName("or_time");
            entity.Property(e => e.OrUId).HasColumnName("or_u_id");

            entity.HasOne(d => d.OrAd).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrAdId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Orders_Addresses_ad_id_fk");

            entity.HasOne(d => d.OrS).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrSId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Orders_Sellers_su_id_fk");

            entity.HasOne(d => d.OrU).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrUId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Orders_Users_u_id_fk");
        });

        modelBuilder.Entity<OrdersHaveProduct>(entity =>
        {
            entity.HasKey(e => e.OhpId).HasName("Orders_have_products_pk");

            entity.ToTable("Orders_have_products");

            entity.Property(e => e.OhpId).HasColumnName("ohp_id");
            entity.Property(e => e.OhpOrId).HasColumnName("ohp_or_id");
            entity.Property(e => e.OhpProId).HasColumnName("ohp_pro_id");
            entity.Property(e => e.OhpQuantity).HasColumnName("ohp_quantity");

            entity.HasOne(d => d.OhpOr).WithMany(p => p.OrdersHaveProducts)
                .HasForeignKey(d => d.OhpOrId)
                .HasConstraintName("Orders_have_products_Orders_or_id_fk");

            entity.HasOne(d => d.OhpPro).WithMany(p => p.OrdersHaveProducts)
                .HasForeignKey(d => d.OhpProId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Orders_have_products_Products_pro_id_fk");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PayId).HasName("Payments_pk");

            entity.Property(e => e.PayId)
                .ValueGeneratedNever()
                .HasColumnName("pay_id");
            entity.Property(e => e.PayMethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("pay_method");
            entity.Property(e => e.PayUserId).HasColumnName("pay_user_id");

            entity.HasOne(d => d.PayUser).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PayUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Payments_Users_u_id_fk");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PId).HasName("permissions_pk");

            entity.HasIndex(e => e.PResource, "permissions_resource_uq").IsUnique();

            entity.Property(e => e.PId).HasColumnName("p_id");
            entity.Property(e => e.PResource)
                .HasMaxLength(128)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("p_resource");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Products_pk");

            entity.Property(e => e.Id).HasColumnName("pro_id");
            entity.Property(e => e.ProCatId).HasColumnName("pro_cat_id");
            entity.Property(e => e.ProCost).HasColumnName("pro_cost");
            entity.Property(e => e.ProDiscount).HasColumnName("pro_discount");
            entity.Property(e => e.ProManufacturer)
                .HasMaxLength(128)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("pro_manufacturer");
            entity.Property(e => e.ProName)
                .HasMaxLength(128)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("pro_name");
            entity.Property(e => e.ProPhotosPath)
                .HasMaxLength(256)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("pro_photos_path");
            entity.Property(e => e.ProQuantity).HasColumnName("pro_quantity");
            entity.Property(e => e.ProRating).HasColumnName("pro_rating");

            entity.HasOne(d => d.ProCat).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProCatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Products_Categories_cat_id_fk");
        });

        modelBuilder.Entity<ProductList>(entity =>
        {
            entity.HasKey(e => e.PlId).HasName("Product_lists_pk");

            entity.ToTable("Product_lists");

            entity.Property(e => e.PlId).HasColumnName("pl_id");
            entity.Property(e => e.PlName)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("pl_name");
            entity.Property(e => e.PlUId).HasColumnName("pl_u_id");

            entity.HasOne(d => d.PlU).WithMany(p => p.ProductLists)
                .HasForeignKey(d => d.PlUId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Product_lists_Users_u_id_fk");
        });

        modelBuilder.Entity<ProductsHaveAttribute>(entity =>
        {
            entity.HasKey(e => e.PhaId).HasName("Products_have_attributes_pk");

            entity.ToTable("Products_have_attributes");

            entity.Property(e => e.PhaId).HasColumnName("pha_id");
            entity.Property(e => e.PhaAtrId).HasColumnName("pha_atr_id");
            entity.Property(e => e.PhaProId).HasColumnName("pha_pro_id");
            entity.Property(e => e.PhaValue)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("pha_value");

            entity.HasOne(d => d.PhaAtr).WithMany(p => p.ProductsHaveAttributes)
                .HasForeignKey(d => d.PhaAtrId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Products_have_attributes_Attributes_atr_id_fk");

            entity.HasOne(d => d.PhaPro).WithMany(p => p.ProductsHaveAttributes)
                .HasForeignKey(d => d.PhaProId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Products_have_attributes_Products_pro_id_fk");
        });

        modelBuilder.Entity<Qiwi>(entity =>
        {
            entity.HasKey(e => e.QiwiId).HasName("Qiwi_pk");

            entity.ToTable("Qiwi");

            entity.Property(e => e.QiwiId)
                .ValueGeneratedNever()
                .HasColumnName("qiwi_id");
            entity.Property(e => e.QiwiNumber).HasColumnName("qiwi_number");
            entity.Property(e => e.QiwiPayId).HasColumnName("qiwi_pay_id");

            entity.HasOne(d => d.QiwiPay).WithMany(p => p.Qiwis)
                .HasForeignKey(d => d.QiwiPayId)
                .HasConstraintName("Qiwi_Payments_pay_id_fk");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.RewId).HasName("Reviews_pk");

            entity.Property(e => e.RewId).HasColumnName("rew_id");
            entity.Property(e => e.RewProId).HasColumnName("rew_pro_id");
            entity.Property(e => e.RewText)
                .IsUnicode(false)
                .HasColumnName("rew_text");
            entity.Property(e => e.RewGrade).HasColumnName("rew_grade");
            entity.Property(e => e.RewUId).HasColumnName("rew_u_id");

            entity.HasOne(d => d.RewPro).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.RewProId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Reviews_Products_pro_id_fk");

            entity.HasOne(d => d.RewU).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.RewUId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Reviews_Users_u_id_fk");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RId).HasName("Roles_pk");

            entity.Property(e => e.RId).HasColumnName("r_id");
            entity.Property(e => e.RName)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("r_name");
        });

        modelBuilder.Entity<RoleHasPermission>(entity =>
        {
            entity.HasKey(e => e.RhpId).HasName("Role_has_permissions_pk");

            entity.ToTable("Role_has_permissions");

            entity.Property(e => e.RhpId).HasColumnName("rhp_id");
            entity.Property(e => e.RhpPermissionId).HasColumnName("rhp_permission_id");
            entity.Property(e => e.RhpRoleId).HasColumnName("rhp_role_id");

            entity.HasOne(d => d.RhpPermission).WithMany(p => p.RoleHasPermissions)
                .HasForeignKey(d => d.RhpPermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Role_has_permissions_Permissions_p_id_fk");

            entity.HasOne(d => d.RhpRole).WithMany(p => p.RoleHasPermissions)
                .HasForeignKey(d => d.RhpRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Role_has_permissions_Roles_r_id_fk");
        });

        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.SuId).HasName("Sellers_pk");

            entity.Property(e => e.SuId)
                .ValueGeneratedNever()
                .HasColumnName("su_id");
            entity.Property(e => e.SName)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("s_name");

            entity.HasOne(d => d.Su).WithOne(p => p.Seller)
                .HasForeignKey<Seller>(d => d.SuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Sellers_Users_u_id_fk");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TrId).HasName("transactions_pk");

            entity.Property(e => e.TrId).HasColumnName("tr_id");
            entity.Property(e => e.TrOrId).HasColumnName("tr_or_id");
            entity.Property(e => e.TrPayId).HasColumnName("tr_pay_id");
            entity.Property(e => e.TrTime)
                .HasColumnType("datetime")
                .HasColumnName("tr_time");

            entity.HasOne(d => d.TrOr).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TrOrId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Transactions_Orders_or_id_fk");

            entity.HasOne(d => d.TrPay).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TrPayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transactions_Payments_pay_id_fk");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pk");

            entity.HasIndex(e => e.UEmail, "Users_email_unique").IsUnique();

            entity.HasIndex(e => e.ULogin, "Users_login_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("u_id");
            entity.Property(e => e.UEmail)
                .HasMaxLength(256)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("u_email");
            entity.Property(e => e.ULogin)
                .HasMaxLength(256)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("u_login");
            entity.Property(e => e.UPassword)
                .HasMaxLength(256)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("u_password");
            entity.Property(e => e.URoleId).HasColumnName("u_role_id");

            entity.HasOne(d => d.URole).WithMany(p => p.Users)
                .HasForeignKey(d => d.URoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Users_Roles_r_id_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
