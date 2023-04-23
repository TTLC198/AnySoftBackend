using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class Category
{
    [Key]
    [Column("cat_id"), Required]
    public int Id { get; set; }
    [Column("cat_parent_id"), Required]
    public int? ParentId { get; set; }
    [Column("cat_name"), Required]
    public string Name { get; set; } = null!;

    [ValidateNever]
    [NotMapped]
    public virtual Category? CatParent { get; set; }
    
    [NotMapped]
    public virtual IEnumerable<CategoriesHaveAttribute> CategoriesHaveAttributes { get; } = new List<CategoriesHaveAttribute>();

    [NotMapped]
    public virtual IEnumerable<Category> InverseCatParent { get; } = new List<Category>();

    [NotMapped]
    public virtual IEnumerable<Product> Products { get; } = new List<Product>();
}
