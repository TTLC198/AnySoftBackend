using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class ProductsHaveProperties
{
    [Key]
    [Column("pha_id"), Required]
    public int Id { get; set; }

    [Column("pha_pro_id"), Required]
    public int ProductId { get; set; }

    [Column("pha_prp_id"), Required]
    public int PropertyId { get; set; }

    [ValidateNever]
    [ForeignKey("PropertyId")]
    public virtual Property? Property { get; }
    [ValidateNever]
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; }
}
