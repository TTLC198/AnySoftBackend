﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_PR_LIB;


public partial class Category
{
    [Column("cat_id")]
    public int Id { get; set; }
    [Column("cat_parent_id")]
    public int? ParentId { get; set; }
    [Column("cat_name")]
    public string Name { get; set; } = null!;

    [ValidateNever]
    public virtual Category? CatParent { get; set; }

    public virtual ICollection<CategoriesHaveAttribute> CategoriesHaveAttributes { get; } = new List<CategoriesHaveAttribute>();

    public virtual ICollection<Category> InverseCatParent { get; } = new List<Category>();

    public virtual ICollection<Product> Products { get; } = new List<Product>();
}
