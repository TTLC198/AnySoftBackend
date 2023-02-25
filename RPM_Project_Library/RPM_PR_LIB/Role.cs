using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;


public partial class Role : BaseModel
{
    [Column("r_id")]   
    public override int Id { get; set; }
    
    [Column("r_name")]
    public string Name { get; set; } = null!;
}
