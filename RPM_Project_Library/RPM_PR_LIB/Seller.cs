using System;
using System.Collections.Generic;

namespace RPM_PR_LIB;


public partial class Seller
{
    public int SuId { get; set; }

    public string SName { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual User Su { get; set; } = null!;
}
