using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;


public partial class Seller
{
    [Column("su_id")]
    public int Id { get; set; }
    [Column("su_name")]
    public string Name { get; set; } = null!;
}
