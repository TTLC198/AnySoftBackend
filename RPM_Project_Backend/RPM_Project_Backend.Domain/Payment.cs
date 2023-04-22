using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RPM_PR_LIB;

namespace RPM_Project_Backend.Domain;


public class Payment
{
    [Column("pay_id")]
    public int Id { get; set; }
    [Column("pay_user_id")]
    public int UserId { get; set; }
    [Column("pay_method")]
    public string Method { get; set; } = null!;
    [Column("pay_is_active")]
    public bool IsActive { get; set; }

    public virtual ICollection<BankCard> BankCards { get; } = new List<BankCard>();
    [ValidateNever]
    public virtual User PayUser { get; set; } = null!;

    public virtual ICollection<Qiwi> Qiwis { get; } = new List<Qiwi>();

    public virtual ICollection<Transaction> Transactions { get; } = new List<Transaction>();
}
