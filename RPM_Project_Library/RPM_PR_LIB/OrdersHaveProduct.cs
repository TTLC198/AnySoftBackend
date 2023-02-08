using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;

public partial class OrdersHaveProduct
{
    [Column("ohp_id")]
    public int Id { get; set; }
    [Column("ohp_pro_id")]
    public int ProductId { get; set; }
    [Column("ohp_or_id")]
    public int? OrderId { get; set; }
    [Column("ohp_quantity")]
    public int Quantity { get; set; }

    public virtual Order? OhpOr { get; set; }

    public virtual Product OhpPro { get; set; } = null!;
}
