using System;
using System.Collections.Generic;

namespace AnySoftBackend.Library.DataTransferObjects.Order;

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
    /// <summary>
    /// Purchased Products
    /// </summary>
    public List<int>? PurchasedProductsIds { get; set; }
}