﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AnySoftBackend.Domain;
/// <summary>
/// Product Object
/// </summary>
public class Product
{
    /// <summary>
    /// Identifier
    /// </summary>
    [Key]
    [Column("pro_id"), Required]
    public int Id { get; set; }
    /// <summary>
    /// Name
    /// </summary>
    [Column("pro_name"), Required, StringLength(128)]
    public string? Name { get; set; }
    /// <summary>
    /// Description
    /// </summary>
    [Column("pro_description"), Required, StringLength(256)]
    public string? Description { get; set; }
    /// <summary>
    /// Product Cost
    /// </summary>
    [Column("pro_cost"), Required]
    public int Cost { get; set; }
    /// <summary>
    /// Product cost
    /// </summary>
    [Column("pro_discount"), Required]
    public int? Discount { get; set; }
    /// <summary>
    /// Product Rating (0 to 5)
    /// </summary>
    [Column("pro_rating")]
    public double Rating { get; set; }
    /// <summary>
    /// Product creation date
    /// </summary>
    [Column("pro_ts"), Required]
    public DateTime Ts { get; set; }
    /// <summary>
    /// Product Seller Identifier
    /// </summary>
    [Column("pro_s_id")]
    public int SellerId { get; set; }

    /// <summary>
    /// Product Seller
    /// </summary>
    [ValidateNever]
    [ForeignKey("SellerId")]
    public virtual User? Seller { get; set; }

    /// <summary>
    /// Product attributes
    /// </summary>
    [ValidateNever]
    public virtual IEnumerable<ProductsHaveProperties>? ProductsHaveProperties { get; }
    
    /// <summary>
    /// Product genres
    /// </summary>
    [ValidateNever]
    public virtual IEnumerable<ProductsHaveGenres>? ProductsHaveGenres { get; }
    
    /// <summary>
    /// Product in orders
    /// </summary>
    [ValidateNever]
    public virtual IEnumerable<OrdersHaveProduct>? OrdersHaveProducts { get; }
    
    /// <summary>
    /// Product in shopping carts
    /// </summary>
    [ValidateNever]
    public virtual IEnumerable<UsersHaveProducts>? UsersHaveProducts { get; }

    /// <summary>
    /// Product images
    /// </summary>
    [ValidateNever]
    public virtual IEnumerable<Image>? Images { get; }
    
    /// <summary>
    /// Product reviews
    /// </summary>
    [ValidateNever]
    public virtual IEnumerable<Review>? Reviews { get; }
}