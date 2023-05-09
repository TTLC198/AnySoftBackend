using System.ComponentModel.DataAnnotations;
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
    [Column("or_u_id"), Required]
    public int UserId { get; set; }
    [Column("or_f_cost"), Required]
    public double FinalCost { get; set; }
    [Column("or_ts"), Required]
    public DateTime Ts { get; set; }
    
    [ValidateNever]
    [ForeignKey("UserId")]
    public virtual User? User { get; }
    [ValidateNever]
    public virtual IEnumerable<OrdersHaveProduct>? OrdersHaveProducts { get; }
    [ValidateNever]
    public virtual IEnumerable<Transaction>? Transactions { get; }
}

/// <summary>
/// Order Data Transfer Object
/// </summary>
public class OrderResponseDto
{
    /// <summary>
    /// Order identifier
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Order Status
    /// </summary>
    public string? Status { get; set; }
    /// <summary>
    /// Order UserId
    /// </summary>
    public int UserId { get; set; }
    /// <summary>
    /// Order Final Cost
    /// </summary>
    public double FinalCost { get; set; }
    /// <summary>
    /// Order Creation Time
    /// </summary>
    public DateTime Ts { get; set; }
}

/// <summary>
/// Order DTO to secure a purchase
/// </summary>
public class OrderPurchaseDto
{
    /// <summary>
    /// Order identifier
    /// </summary>
    public int OrderId { get; set; }
    /// <summary>
    /// Payment identifier
    /// </summary>
    public int PaymentId { get; set; }
}


