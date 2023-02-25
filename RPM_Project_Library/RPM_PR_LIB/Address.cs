﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RPM_PR_LIB;

public partial class Address
{
    [Column("ad_id"), Required]
    public int Id { get; set; }
    [Column("ad_street"), Required]
    public string Street { get; set; } = null!;
    [Column("ad_city"), Required]
    public string City { get; set; } = null!;
    [Column("ad_state"), Required]
    public string State { get; set; } = null!;
    [Column("ad_country"), Required]
    public string Country { get; set; } = null!;
    [Column("ad_u_id"), Required]
    public int UserId { get; set; }
    [Column("ad_zip"), Required]
    public int Zip { get; set; }
    [Column("ad_is_active"), Required]
    public bool IsActive { get; set; }
    
    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual User User { get; set; } = null!;
}
