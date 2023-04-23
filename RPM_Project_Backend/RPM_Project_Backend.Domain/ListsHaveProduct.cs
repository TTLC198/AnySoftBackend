using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;


public class ListsHaveProduct
{
    [Key]
    [Column("lhp_id"), Required]
    public int Id { get; set; }
    [Column("lhp_pl_id"), Required]
    public int ProductListId { get; set; }
    [Column("lhp_pro_id"), Required]
    public int ProductId { get; set; }
    [Column("lhp_quantity"), Required]
    public int Quantity { get; set; }
    
    [ValidateNever]
    public virtual ProductList LhpPl { get; set; } = null!;
    [ValidateNever]
    public virtual Product LhpPro { get; set; } = null!;
}
