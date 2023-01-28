using System;
using System.Collections.Generic;

namespace RPM_PR_LIB;


public partial class ProductList
{
    public int PlId { get; set; }

    public int PlUId { get; set; }

    public string PlName { get; set; } = null!;

    public virtual ICollection<ListsHaveProduct> ListsHaveProducts { get; } = new List<ListsHaveProduct>();

    public virtual User PlU { get; set; } = null!;
}
