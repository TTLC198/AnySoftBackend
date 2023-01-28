using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;

public partial class Product : BaseModel
{
    [Column("pro_id")]
    public override int Id { get; set; }

    public string ProName { get; set; } = null!;

    public int ProQuantity { get; set; }

    public int ProCost { get; set; }

    public int? ProDiscount { get; set; }

    public int ProCatId { get; set; }

    public string ProManufacturer { get; set; } = null!;

    public string ProPhotosPath { get; set; } = null!;

    public double ProRating { get; set; }

    public virtual ICollection<ListsHaveProduct> ListsHaveProducts { get; } = new List<ListsHaveProduct>();

    public virtual ICollection<OrdersHaveProduct> OrdersHaveProducts { get; } = new List<OrdersHaveProduct>();

    public virtual Category ProCat { get; set; } = null!;

    public virtual ICollection<ProductsHaveAttribute> ProductsHaveAttributes { get; } = new List<ProductsHaveAttribute>();

    public virtual ICollection<Review> Reviews { get; } = new List<Review>();
}
