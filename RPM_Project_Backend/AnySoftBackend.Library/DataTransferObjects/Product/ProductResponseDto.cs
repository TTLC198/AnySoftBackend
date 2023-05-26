using System;
using System.Collections.Generic;
using AnySoftBackend.Library.DataTransferObjects;
using AnySoftBackend.Library.DataTransferObjects.Property;
using AnySoftBackend.Library.DataTransferObjects.Review;
using AnySoftBackend.Library.DataTransferObjects.User;

namespace RPM_Project_Backend.Domain;

/// <summary>
/// Product object that is returned when requested
/// </summary>
public class ProductResponseDto
{
    /// <summary>
    /// Identifier
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Name
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// Product Cost
    /// </summary>
    public int Cost { get; set; }
    /// <summary>
    /// Product cost
    /// </summary>
    public int? Discount { get; set; }
    /// <summary>
    /// Product Rating (0 to 5)
    /// </summary>
    public double Rating { get; set; }
    /// <summary>
    /// Product creation date
    /// </summary>
    public DateTime Ts { get; set; }

    /// <summary>
    /// Product Seller
    /// </summary>
    public virtual UserResponseDto? Seller { get; set; }

    /// <summary>
    /// Product images
    /// </summary>
    public virtual IEnumerable<string>? Images { get; set; }
    
    /// <summary>
    /// Product reviews
    /// </summary>
    public virtual IEnumerable<ReviewResponseDto>? Reviews { get; set; }
    
    /// <summary>
    /// Product Properties
    /// </summary>
    public virtual IEnumerable<PropertyDto>? Properties { get; set; }
    
    /// <summary>
    /// Product Genres
    /// </summary>
    public virtual IEnumerable<GenreDto>? Genres { get; set; }
}