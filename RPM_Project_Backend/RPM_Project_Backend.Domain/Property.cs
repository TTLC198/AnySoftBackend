using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace RPM_Project_Backend.Domain;

[Index(nameof(Name), IsUnique = true)]
public class Property
{
    [Key]
    [Column("atr_id"), Required]
    public int Id { get; set; }
    [Column("atr_name"), Required, StringLength(50)]
    public string? Name { get; set; }
    [Column("atr_type"), Required, StringLength(50)]
    public string? Type { get; set; }

    [ValidateNever]
    public virtual IEnumerable<ProductsHaveProperties>? ProductsHaveProperties { get; }
}