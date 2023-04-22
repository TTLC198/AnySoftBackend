using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RPM_PR_LIB;

namespace RPM_Project_Backend.Domain;


public class ListsHaveProduct
{
    [Column("lhp_id")] //регистр важен? в бд L
    public int Id { get; set; }
    [Column("lhp_pl_id")]
    public int ProductListId { get; set; }
    [Column("lhp_pro_id")]
    public int ProductId { get; set; }
    [Column("lhp_quantity")]
    public int Quantity { get; set; }
    
    [ValidateNever]
    public virtual ProductList LhpPl { get; set; } = null!;
    [ValidateNever]
    public virtual Product LhpPro { get; set; } = null!;
}
