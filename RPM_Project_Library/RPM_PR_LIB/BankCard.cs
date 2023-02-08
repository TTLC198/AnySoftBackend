using System;
using System.Collections.Generic;

namespace RPM_PR_LIB;


public partial class BankCard
{
    public int BcNumber { get; set; }

    public string BcName { get; set; } = null!;

    public DateTime BcExpirationDate { get; set; }

    public int BcCvc { get; set; }

    public int BcPaymentId { get; set; }

    public int BcId { get; set; }

    public virtual Payment BcPayment { get; set; } = null!;
}
