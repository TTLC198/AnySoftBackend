using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;
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
    [Column("pro_name"), Required]
    public string Name { get; set; } = null!;
    /// <summary>
    /// Product Quantity
    /// </summary>
    [Column("pro_quantity"), Required]
    public int Quantity { get; set; }
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
    /// Product Category Identifier
    /// </summary>
    [Column("pro_cat_id"), Required]
    public int CategoryId { get; set; }
    /// <summary>
    /// Product Rating (0 to 5)
    /// </summary>
    [Column("pro_rating")]
    public double Rating { get; set; }

    /// <summary>
    /// Product Category 
    /// </summary>
    [ValidateNever]
    public virtual Category Category { get; set; } = null!;
    /// <summary>
    /// Product Seller Identifier
    /// </summary>
    [Column("pro_s_id")]
    public int SellerId { get; set; }
    /// <summary>
    /// Product Seller
    /// </summary>
    [ValidateNever]
    public virtual User Seller { get; set; } = null!;
    /// <summary>
    /// Product attributes
    /// </summary>
    [ValidateNever]
    public virtual IEnumerable<ProductsHaveAttribute> ProductsHaveAttributes { get; } = new List<ProductsHaveAttribute>();
    /// <summary>
    /// Product reviews
    /// </summary>
    [ValidateNever]
    public virtual IEnumerable<Review> Reviews { get; } = new List<Review>();
}
/// <summary>
/// Product Data Transfer Object
/// </summary>
public class ProductDto
{
    /// <summary>
    /// Product Name
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required!")]
    public string Name { get; set; } = null!;
    /// <summary>
    /// Product Cost
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required!")]
    public int Cost { get; set; }
    /// <summary>
    /// Discount
    /// </summary>
    public int? Discount { get; set; }
    /// <summary>
    /// Category Identifier
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Category is required!")]
    public int CategoryId { get; set; }
}
