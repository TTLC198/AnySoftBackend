using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;


public class Order
{
    [Column("or_id"), Required]
    public int Id { get; set; }
    [Column("or_number"), Required]
    public int Number { get; set; }
    [Column("or_status"), Required, StringLength(50)]
    public string? Status { get; set; }
    [Column("or_ad_id"), Required]
    public int AddressId { get; set; }
    [Column("or_u_id"), Required]
    public int UserId { get; set; }
    [Column("or_f_cost"), Required]
    public double FinalCost { get; set; }
    [Column("or_time"), Required]
    public DateTime Time { get; set; }

    [ValidateNever]
    public virtual Address? Address { get; set; }
    [ValidateNever]
    public virtual User? User { get; set; }

    public virtual IEnumerable<OrdersHaveProduct>? OrdersHaveProducts { get; }

    public virtual IEnumerable<Transaction>? Transactions { get; }
}
