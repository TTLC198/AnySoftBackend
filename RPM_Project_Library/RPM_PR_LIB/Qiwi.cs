using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_PR_LIB;


public partial class Qiwi
{
    [Column("qiwi_id")]
    public int Id { get; set; }
    
    [Column("qiwi_number")]
    public int Number { get; set; }
    
    [Column("qiwi_pay_id")]
    public int PayId { get; set; }
    [ValidateNever]
    public virtual Payment Payment { get; set; }
}
