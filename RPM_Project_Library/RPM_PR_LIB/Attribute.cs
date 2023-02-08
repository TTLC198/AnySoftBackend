using System;
using System.Collections.Generic;

namespace RPM_PR_LIB;

public partial class Attribute
{
    public int AtrId { get; set; }

    public string AtrName { get; set; } = null!;

    public virtual ICollection<CategoriesHaveAttribute> CategoriesHaveAttributes { get; } = new List<CategoriesHaveAttribute>();

    public virtual ICollection<ProductsHaveAttribute> ProductsHaveAttributes { get; } = new List<ProductsHaveAttribute>();
}
