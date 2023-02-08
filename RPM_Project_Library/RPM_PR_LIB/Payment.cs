using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;


public partial class Payment
{
    [Column("pay_id")]
    public int Id { get; set; }
    [Column("pay_user_id")]
    public int UserId { get; set; }
    [Column("pay_method")]
    public string Method { get; set; } = null!;

    public virtual ICollection<BankCard> BankCards { get; } = new List<BankCard>();

    public virtual User PayUser { get; set; } = null!;

    public virtual ICollection<Qiwi> Qiwis { get; } = new List<Qiwi>();

    public virtual ICollection<Transaction> Transactions { get; } = new List<Transaction>();
}
