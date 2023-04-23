using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RPM_Project_Backend.Domain;

[Index(nameof(Name), IsUnique = true)]
public class Attribute
{
    [Key]
    [Column("atr_id"), Required]
    public int Id { get; set; }
    [Column("atr_name"), Required]
    public string? Name { get; set; }
    [Column("atr_type"), Required]
    public string? Type { get; set; }

    public virtual IEnumerable<CategoriesHaveAttribute> CategoriesHaveAttributes { get; } =
        new List<CategoriesHaveAttribute>();

    public virtual IEnumerable<ProductsHaveAttribute> ProductsHaveAttributes { get; } =
        new List<ProductsHaveAttribute>();
}