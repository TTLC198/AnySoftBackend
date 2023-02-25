using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;


public partial class ListsHaveProduct
{
    [Column("lhp_id")] //регистр важен? в бд L
    public int Id { get; set; }
    [Column("lhp_pl_id")]
    public int ProductListId { get; set; }
    [Column("lhp_pro_id")]
    public int ProductId { get; set; }
    [Column("lhp_quantity")]
    public int Quantity { get; set; }

    public virtual ProductList LhpPl { get; set; } = null!;

    public virtual Product LhpPro { get; set; } = null!;
}
