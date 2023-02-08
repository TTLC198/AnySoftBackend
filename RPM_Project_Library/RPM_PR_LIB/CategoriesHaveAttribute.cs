using System;
using System.Collections.Generic;

namespace RPM_PR_LIB;


public partial class CategoriesHaveAttribute
{
    public int ChaId { get; set; }

    public int ChaCatId { get; set; }

    public int ChaAtrId { get; set; }

    public virtual Attribute ChaAtr { get; set; } = null!;

    public virtual Category ChaCat { get; set; } = null!;
}
