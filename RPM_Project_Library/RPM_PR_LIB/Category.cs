using System;
using System.Collections.Generic;

namespace RPM_PR_LIB;


public partial class Category
{
    public int CatId { get; set; }

    public int? CatParentId { get; set; }

    public string CatName { get; set; } = null!;

    public virtual Category? CatParent { get; set; }

    public virtual ICollection<CategoriesHaveAttribute> CategoriesHaveAttributes { get; } = new List<CategoriesHaveAttribute>();

    public virtual ICollection<Category> InverseCatParent { get; } = new List<Category>();

    public virtual ICollection<Product> Products { get; } = new List<Product>();
}
