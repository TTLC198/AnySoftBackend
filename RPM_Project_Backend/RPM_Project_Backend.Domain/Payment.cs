using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;


public class Payment
{
    [Key]
    [Column("pay_id"), Required]
    public int Id { get; set; }
    [Column("pay_user_id"), Required]
    public int UserId { get; set; }
    [Column("pay_method"), Required, StringLength(50)]
    public string? Method { get; set; }
    [Column("pay_is_active"), Required]
    public bool IsActive { get; set; }
    
    [ValidateNever]
    public virtual IEnumerable<BankCard>? BankCards { get; }
    [ValidateNever]
    [ForeignKey("pay_user_id")]
    public virtual User? User { get; set; }
    [ValidateNever]
    public virtual IEnumerable<Qiwi>? Qiwis { get; }
    [ValidateNever]
    public virtual IEnumerable<Transaction>? Transactions { get; }
}
