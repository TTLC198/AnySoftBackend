using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AnySoftBackend.Domain;

public class ProductsHaveProperties
{
    [Key]
    [Column("php_id"), Required]
    public int Id { get; set; }

    [Column("php_pro_id"), Required]
    public int ProductId { get; set; }

    [Column("php_prp_id"), Required]
    public int PropertyId { get; set; }

    [ValidateNever]
    [ForeignKey("PropertyId")]
    public virtual Property? Property { get; set; }
    [ValidateNever]
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
}
