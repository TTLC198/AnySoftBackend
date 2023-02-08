using System;
using System.Collections.Generic;


namespace RPM_PR_LIB;

public partial class Address
{
    public int AdId { get; set; }

    public string AdStreet { get; set; } = null!;

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string Country { get; set; } = null!;

    public int AdUId { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
