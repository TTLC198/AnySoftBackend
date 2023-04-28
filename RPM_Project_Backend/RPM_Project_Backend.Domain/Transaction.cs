using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class Transaction
{
    [Key]
    [Column("tr_id"), Required]
    public int Id { get; set; }

    [Column("tr_pay_id"), Required]
    public int PaymentId { get; set; }

    [Column("tr_order_id"), Required]
    public int OrderId { get; set; }

    [Column("tr_time"), Required]
    public DateTime Time { get; set; }

    [ValidateNever]
    [ForeignKey("tr_order_id")]
    public virtual Order? Order { get; set; }
    [ValidateNever]
    [ForeignKey("tr_pay_id")]
    public virtual Payment? Payment { get; set; }
}
