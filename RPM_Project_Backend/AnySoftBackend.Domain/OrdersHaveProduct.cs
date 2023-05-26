using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AnySoftBackend.Domain;

public class OrdersHaveProduct
{
    [Key]
    [Column("ohp_id"), Required]
    public int Id { get; set; }
    [Column("ohp_pro_id"), Required]
    public int ProductId { get; set; }
    [Column("ohp_or_id"), Required]
    public int OrderId { get; set; }

    [ValidateNever]
    [ForeignKey("OrderId")]
    public virtual Order? Order { get; }
    [ValidateNever]
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; }
}
