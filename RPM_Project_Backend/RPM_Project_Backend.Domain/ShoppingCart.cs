using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

/// <summary>
/// Shopping Cart Data Transfer Object
/// </summary>
public class ShoppingCartDto
{
    /// <summary>
    /// Product IDs to add to the database
    /// </summary>
    public IEnumerable<int> ProductIds { get; set; }
}

/// <summary>
/// Shopping Cart Response model
/// </summary>
public class ShoppingCartResponseDto
{
    /// <summary>
    /// Product IDs from the database
    /// </summary>
    public IEnumerable<ProductResponseDto> Products { get; set; }
}
