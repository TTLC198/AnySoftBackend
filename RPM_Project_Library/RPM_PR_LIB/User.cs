using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;

public partial class User : BaseModel
{
    public string ULogin { get; set; } = null!;

    public string UPassword { get; set; } = null!;

    public string UEmail { get; set; } = null!;

    public int URoleId { get; set; }

    [Column("u_id")]
    public override int Id { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual ICollection<Payment> Payments { get; } = new List<Payment>();

    public virtual ICollection<ProductList> ProductLists { get; } = new List<ProductList>();

    public virtual ICollection<Review> Reviews { get; } = new List<Review>();

    public virtual Seller? Seller { get; set; }

    public virtual Role URole { get; set; } = null!;
}
