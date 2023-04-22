﻿using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RPM_PR_LIB;

namespace RPM_Project_Backend.Domain;

public class OrdersHaveProduct
{
    [Column("ohp_id")]
    public int Id { get; set; }
    [Column("ohp_pro_id")]
    public int ProductId { get; set; }
    [Column("ohp_or_id")]
    public int OrderId { get; set; } // почему необязательное значение?
    [Column("ohp_quantity")]
    public int Quantity { get; set; }
    [ValidateNever]
    public virtual Order OhpOr { get; set; }
    [ValidateNever]
    public virtual Product OhpPro { get; set; } = null!;
}