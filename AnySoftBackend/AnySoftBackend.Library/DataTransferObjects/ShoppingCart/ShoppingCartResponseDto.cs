using System.Collections.Generic;
using AnySoftBackend.Library.DataTransferObjects.Product;

namespace AnySoftBackend.Library.DataTransferObjects.ShoppingCart;

/// <summary>
/// Shopping Cart Response model
/// </summary>
public class ShoppingCartResponseDto
{
    /// <summary>
    /// Product IDs from the database
    /// </summary>
    public IEnumerable<ProductResponseDto>? Products { get; set; }
}