using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using RPM_PR_LIB;

namespace RPM_Project_Backend.Domain;

[Index(nameof(Name), IsUnique = true)]
public class Attribute
{
    [Column("atr_id"), Required] public int Id { get; set; }
    [Column("atr_name"), Required] public string Name { get; set; } = null!;
    [Column("atr_type"), Required] public string Type { get; set; } = null!;

    public virtual ICollection<CategoriesHaveAttribute> CategoriesHaveAttributes { get; } =
        new List<CategoriesHaveAttribute>();

    public virtual ICollection<ProductsHaveAttribute> ProductsHaveAttributes { get; } =
        new List<ProductsHaveAttribute>();
}