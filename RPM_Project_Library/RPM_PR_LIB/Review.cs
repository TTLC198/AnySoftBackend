using System;
using System.Collections.Generic;

namespace RPM_PR_LIB;


public partial class Review
{
    public int RewId { get; set; }

    public int RewUId { get; set; }

    public string RewText { get; set; } = null!;
    
    public float RewGrade { get; set; }

    public int RewProId { get; set; }

    public virtual Product RewPro { get; set; } = null!;

    public virtual User RewU { get; set; } = null!;
}
