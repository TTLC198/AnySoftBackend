using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AnySoftBackend.Domain;

public class Transaction
{
    [Key]
    [Column("tr_id"), Required]
    public int Id { get; set; }

    [Column("tr_pay_id"), Required]
    public int PaymentId { get; set; }

    [Column("tr_order_id"), Required]
    public int OrderId { get; set; }

    [ValidateNever]
    [ForeignKey("OrderId")]
    public virtual Order? Order { get; }
    [ValidateNever]
    [ForeignKey("PaymentId")]
    public virtual Payment? Payment { get; }
}
