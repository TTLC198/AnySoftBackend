using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class UsersHaveProducts
{
    [Key]
    [Column("uhp_id"), Required]
    public int Id { get; set; }
    [Column("uhp_pl_id"), Required]
    public int UserId { get; set; }
    [Column("uhp_pro_id"), Required]
    public int ProductId { get; set; }
    [Column("uhp_quantity"), Required]
    public int Quantity { get; set; }
    
    [ValidateNever]
    [ForeignKey("UserId")]
    public virtual User? User { get; }
    [ValidateNever]
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; }
}
