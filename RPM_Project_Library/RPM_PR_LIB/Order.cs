using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;


public partial class Order
{
    [Column("or_id")]
    public int Id { get; set; }
    [Column("or_number")]
    public int Number { get; set; }
    [Column("or_status")]
    public string Status { get; set; } = null!;
    [Column("or_ad_id")]
    public int AddressId { get; set; }
    [Column("or_u_id")]
    public int UserId { get; set; }
    [Column("or_s_id")]
    public int SellerId { get; set; }
    [Column("or_f_cost")]
    public double FloatCost { get; set; }
    [Column("or_time")]
    public DateTime Time { get; set; }

    public virtual Address OrAd { get; set; } = null!;

    public virtual User OrS { get; set; } = null!;

    public virtual User OrU { get; set; } = null!;

    public virtual ICollection<OrdersHaveProduct> OrdersHaveProducts { get; } = new List<OrdersHaveProduct>();

    public virtual ICollection<Transaction> Transactions { get; } = new List<Transaction>();
}
