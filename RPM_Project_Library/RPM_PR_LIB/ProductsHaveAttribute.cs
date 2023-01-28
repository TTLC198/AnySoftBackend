using System;
using System.Collections.Generic;

namespace RPM_PR_LIB;


public partial class ProductsHaveAttribute
{
    public int PhaId { get; set; }

    public int PhaProId { get; set; }

    public int PhaAtrId { get; set; }

    public string PhaValue { get; set; } = null!;

    public virtual Attribute PhaAtr { get; set; } = null!;

    public virtual Product PhaPro { get; set; } = null!;
}
