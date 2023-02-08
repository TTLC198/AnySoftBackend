using System;
using System.Collections.Generic;

namespace RPM_PR_LIB;

public partial class OrdersHaveProduct
{
    public int OhpId { get; set; }

    public int OhpProId { get; set; }

    public int? OhpOrId { get; set; }

    public int OhpQuantity { get; set; }

    public virtual Order? OhpOr { get; set; }

    public virtual Product OhpPro { get; set; } = null!;
}
