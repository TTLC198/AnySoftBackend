using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RPM_PR_LIB;

namespace RPM_Project_Backend.Domain;

public class Category
{
    [Column("cat_id")]
    public int Id { get; set; }
    [Column("cat_parent_id")]
    public int? ParentId { get; set; }
    [Column("cat_name")]
    public string Name { get; set; } = null!;

    [ValidateNever]
    [NotMapped]
    public virtual Category? CatParent { get; set; }
    
    [NotMapped]
    public virtual ICollection<CategoriesHaveAttribute> CategoriesHaveAttributes { get; } = new List<CategoriesHaveAttribute>();

    [NotMapped]
    public virtual ICollection<Category> InverseCatParent { get; } = new List<Category>();

    [NotMapped]
    public virtual ICollection<Product> Products { get; } = new List<Product>();
}
