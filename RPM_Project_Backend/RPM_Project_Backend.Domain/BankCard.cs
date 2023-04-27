using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class BankCard
{
    [Key]
    [Column("bc_id"), Required] 
    public int Id { get; set; }
    [Column("bc_number"), Required] 
    public int Number { get; set; }
    [Column("bc_name"), Required, StringLength(50)] 
    public string? Name { get; set; }
    [Column("bc_expiration_date"), Required]
    public DateTime ExpirationDate { get; set; }
    [Column("bc_cvc"), Required] 
    public int Cvc { get; set; }
    [Column("bc_payment_id"), Required] 
    public int PaymentId { get; set; }

    [ValidateNever]
    public virtual Payment? BankCardPayment { get; set; }
}