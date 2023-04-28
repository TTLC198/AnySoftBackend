using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class OrdersHaveProduct
{
    [Key]
    [Column("ohp_id"), Required]
    public int Id { get; set; }
    [Column("ohp_pro_id"), Required]
    public int ProductId { get; set; }
    [Column("ohp_or_id"), Required]
    public int OrderId { get; set; }
    [Column("ohp_quantity"), Required]
    public int Quantity { get; set; }
    
    [ValidateNever]
    [ForeignKey("ohp_or_id")]
    public virtual Order? Order { get; set; }
    [ValidateNever]
    [ForeignKey("ohp_pro_id")]
    public virtual Product? Product { get; set; }
}
