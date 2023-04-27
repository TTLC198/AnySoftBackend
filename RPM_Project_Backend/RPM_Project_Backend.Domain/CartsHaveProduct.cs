using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class CartsHaveProduct
{
    [Key]
    [Column("lhp_id"), Required]
    public int Id { get; set; }
    [Column("lhp_pl_id"), Required]
    public int ShoppingCartId { get; set; }
    [Column("lhp_pro_id"), Required]
    public int ProductId { get; set; }
    [Column("lhp_quantity"), Required]
    public int Quantity { get; set; }
    
    [ValidateNever]
    public virtual ShoppingCart? ShoppingCart { get; set; }
    [ValidateNever]
    public virtual Product? Product { get; set; }
}
