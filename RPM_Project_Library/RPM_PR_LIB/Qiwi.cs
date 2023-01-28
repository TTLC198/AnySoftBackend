using System;
using System.Collections.Generic;

namespace RPM_PR_LIB;


public partial class Qiwi
{
    public int QiwiId { get; set; }

    public int? QiwiNumber { get; set; }

    public int? QiwiPayId { get; set; }

    public virtual Payment? QiwiPay { get; set; }
}
