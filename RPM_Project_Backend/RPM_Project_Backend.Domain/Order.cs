﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;


public class Order
{
    [Key]
    [Column("or_id"), Required]
    public int Id { get; set; }
    [Column("or_status"), Required, StringLength(128)]
    public string? Status { get; set; }
    [Column("or_ad_id"), Required]
    public int AddressId { get; set; }
    [Column("or_u_id"), Required]
    public int UserId { get; set; }
    [Column("or_f_cost"), Required]
    public double FinalCost { get; set; }
    [Column("or_time"), Required]
    public DateTime Time { get; set; }

    [ValidateNever]
    [ForeignKey("AddressId")]
    public virtual Address? Address { get; }
    [ValidateNever]
    [ForeignKey("UserId")]
    public virtual User? User { get; }
    [ValidateNever]
    public virtual IEnumerable<OrdersHaveProduct>? OrdersHaveProducts { get; }
    [ValidateNever]
    public virtual IEnumerable<Transaction>? Transactions { get; }
}
