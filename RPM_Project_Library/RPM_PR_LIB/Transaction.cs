using System;
using System.Collections.Generic;

namespace RPM_PR_LIB;

public partial class Transaction
{
    public int TrId { get; set; }

    public int TrPayId { get; set; }

    public int TrOrId { get; set; }

    public DateTime TrTime { get; set; }

    public virtual Order TrOr { get; set; } = null!;

    public virtual Payment TrPay { get; set; } = null!;
}
