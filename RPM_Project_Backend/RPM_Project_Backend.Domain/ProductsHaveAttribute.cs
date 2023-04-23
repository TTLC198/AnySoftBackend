using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class ProductsHaveAttribute 
{
    [Key]
    [Column("pha_id"), Required]
    public int Id { get; set; }

    [Column("pha_pro_id"), Required]
    public int ProductId { get; set; }

    [Column("pha_atr_id"), Required]
    public int AttributeId { get; set; }

    [Column("pha_value"), Required]
    public string? Value { get; set; }

    [ValidateNever]
    public virtual Attribute? Attribute { get; set; }
    [ValidateNever]
    public virtual Product? Product { get; set; }
}
