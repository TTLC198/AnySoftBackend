using System.Collections.Generic;
using AnySoftBackend.Library.DataTransferObjects.Order;
using AnySoftBackend.Library.DataTransferObjects.Product;

namespace AnySoftBackend.Library.DataTransferObjects.User;

/// <summary>
/// User object that is returned when requested
/// </summary>
public class UserResponseDto
{
    /// <summary>
    /// Identifier
    /// </summary>
    public int? Id { get; set; }
    /// <summary>
    /// Login
    /// </summary>
    public string? Login { get; set; }
    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }
    /// <summary>
    /// Image Path
    /// </summary>
    public string? Image { get; set; }
    /// <summary>
    /// User Product IDs in Shopping Cart
    /// </summary>
    public IEnumerable<ProductResponseDto>? ShoppingCart { get; set; }
    /// <summary>
    /// User orders
    /// </summary>
    public IEnumerable<OrderResponseDto>? Orders { get; set; }
}