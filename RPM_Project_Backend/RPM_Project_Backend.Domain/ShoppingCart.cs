using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class ShoppingCart
{
    [Key]
    [Column("sc_id"), Required]
    public int Id { get; set; }

    [Column("sc_u_id"), Required]
    public int UserId { get; set; }

    [ValidateNever]
    [ForeignKey("UserId")]
    public virtual User? User { get; }
    
    [ValidateNever]
    public virtual IEnumerable<CartsHaveProduct>? CartsHaveProducts { get; }
}
