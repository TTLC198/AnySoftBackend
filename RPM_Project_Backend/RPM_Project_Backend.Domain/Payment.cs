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
    [Column("pay_number"), Required, StringLength(24)] 
    public string? Number { get; set; }
    [Column("pay_expiration_date"), Required]
    public DateTime ExpirationDate { get; set; }
    [Column("pay_cvc"), Required, StringLength(4)] 
    public string? Cvc { get; set; }
    [Column("pay_is_active"), Required]
    public bool IsActive { get; set; }
    
    [ValidateNever]
    [ForeignKey("UserId")]
    public virtual User? User { get; }
    [ValidateNever]
    public virtual IEnumerable<Transaction>? Transactions { get; }
}
