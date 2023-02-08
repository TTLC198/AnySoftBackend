using System;
using System.Collections.Generic;

namespace RPM_PR_LIB;


public partial class Order
{
    public int OrId { get; set; }

    public int OrNumber { get; set; }

    public string OrStatus { get; set; } = null!;

    public int OrAdId { get; set; }

    public int OrUId { get; set; }

    public int OrSId { get; set; }

    public double OrFcost { get; set; }

    public DateTime OrTime { get; set; }

    public virtual Address OrAd { get; set; } = null!;

    public virtual Seller OrS { get; set; } = null!;

    public virtual User OrU { get; set; } = null!;

    public virtual ICollection<OrdersHaveProduct> OrdersHaveProducts { get; } = new List<OrdersHaveProduct>();

    public virtual ICollection<Transaction> Transactions { get; } = new List<Transaction>();
}
