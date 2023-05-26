using System.Collections.Generic;

namespace AnySoftBackend.Library.DataTransferObjects.ShoppingCart;

/// <summary>
/// Shopping Cart Data Transfer Object
/// </summary>
public class ShoppingCartCreateDto
{
    /// <summary>
    /// Product IDs to add to the database
    /// </summary>
    public IEnumerable<int>? ProductIds { get; set; }
}