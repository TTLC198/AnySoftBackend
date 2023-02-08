using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;

public partial class Product : BaseModel
{
    [Column("pro_id")]
    public override int Id { get; set; }
    [Column("pro_id")]
    public string Name { get; set; } = null!;
    
    [Column("pro_quantity")]
    public int Quantity { get; set; }
    
    [Column("pro_cost")]
    public int Cost { get; set; }

    [Column("pro_discount")]
    public int? Discount { get; set; }

    [Column("pro_cat_id")]
    public int CatId { get; set; }

    public virtual Category Category { get; set; } = null!;

    [Column("pro_s_id")]
    public string SellerId { get; set; }
    
    public virtual User Seller { get; set; } = null!; //Does it need "=null!"? What is its purpose?

    [Column("pro_photos_path")]
    public string PhotosPath { get; set; } = null!;

    [Column("pro_rating")]
    public double Rating { get; set; }
    
    [Column("pro_id")]
    public virtual ICollection<ProductsHaveAttribute> ProductsHaveAttributes { get; } = new List<ProductsHaveAttribute>();
    [Column("pro_id")]
    public virtual ICollection<Review> Reviews { get; } = new List<Review>();
}
