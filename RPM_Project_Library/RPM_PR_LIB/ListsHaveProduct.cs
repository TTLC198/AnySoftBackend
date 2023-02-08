using System;
using System.Collections.Generic;

namespace RPM_PR_LIB;


public partial class ListsHaveProduct
{
    public int LhpId { get; set; }

    public int LhpPlId { get; set; }

    public int LhpProId { get; set; }

    public virtual ProductList LhpPl { get; set; } = null!;

    public virtual Product LhpPro { get; set; } = null!;
}
