using System;
using System.Collections.Generic;

namespace RPM_PR_LIB;


public partial class Payment
{
    public int PayId { get; set; }

    public int PayUserId { get; set; }

    public string PayMethod { get; set; } = null!;

    public virtual ICollection<BankCard> BankCards { get; } = new List<BankCard>();

    public virtual User PayUser { get; set; } = null!;

    public virtual ICollection<Qiwi> Qiwis { get; } = new List<Qiwi>();

    public virtual ICollection<Transaction> Transactions { get; } = new List<Transaction>();
}
