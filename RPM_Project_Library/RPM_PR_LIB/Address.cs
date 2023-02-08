using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace RPM_PR_LIB;

public partial class Address
{
    [Column("ad_id")]
    public int Id { get; set; }
    [Column("ad_street")]
    public string Street { get; set; } = null!;
    [Column("ad_city")]
    public string City { get; set; } = null!;
    [Column("ad_state")]
    public string State { get; set; } = null!;
    [Column("ad_country")]
    public string Country { get; set; } = null!;
    [Column("ad_u_id")]
    public int UserId { get; set; }
    [Column("ad_zip")]
    public int Zip { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
