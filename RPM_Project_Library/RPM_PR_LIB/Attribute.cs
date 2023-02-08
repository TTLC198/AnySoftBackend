using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;

public partial class Attribute
{
    [Column("atr_id")]
    public int Id { get; set; }
    [Column("atr_name")]
    public string Name { get; set; } = null!;

    public virtual ICollection<CategoriesHaveAttribute> CategoriesHaveAttributes { get; } = new List<CategoriesHaveAttribute>();

    public virtual ICollection<ProductsHaveAttribute> ProductsHaveAttributes { get; } = new List<ProductsHaveAttribute>();
}
