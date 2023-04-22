using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RPM_PR_LIB;

namespace RPM_Project_Backend.Domain;

public class BankCard
{
    [Column("bc_number")] 
    public int Number { get; set; }
    [Column("bc_name")] 
    public string Name { get; set; } = null!;
    [Column("bc_expiration_date")]
    public DateTime ExpirationDate { get; set; }
    [Column("bc_cvc")] 
    public int Cvc { get; set; }
    [Column("bc_payment_id")] 
    public int PaymentId { get; set; }
    [Column("bc_id")] 
    public int Id { get; set; }

    [ValidateNever]
    public virtual Payment BcPayment { get; set; } = null!;
}