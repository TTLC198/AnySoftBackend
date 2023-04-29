using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class CartsHaveProducts
{
    [Key]
    [Column("chp_id"), Required]
    public int Id { get; set; }
    [Column("chp_pl_id"), Required]
    public int ShoppingCartId { get; set; }
    [Column("chp_pro_id"), Required]
    public int ProductId { get; set; }
    [Column("chp_quantity"), Required]
    public int Quantity { get; set; }
    
    [ValidateNever]
    [ForeignKey("ShoppingCartId")]
    public virtual ShoppingCart? ShoppingCart { get; }
    [ValidateNever]
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; }
}
