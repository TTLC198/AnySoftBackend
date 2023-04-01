using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_PR_LIB;

public partial class Transaction
{
    [Column("tr_id")]
    public int Id { get; set; }

    [Column("tr_pay_id")]
    public int PaymentId { get; set; }
    [ValidateNever]
    public virtual Payment Payment { get; set; } = null!;

    [Column("tr_order_id")]
    public int OrderId { get; set; }
    [ValidateNever]
    public virtual Order Order { get; set; } = null!;

    [Column("tr_time")]
    public DateTime Time { get; set; }
}
