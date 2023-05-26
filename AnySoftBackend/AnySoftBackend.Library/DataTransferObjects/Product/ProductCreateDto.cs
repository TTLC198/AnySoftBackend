using System.ComponentModel.DataAnnotations;

namespace AnySoftBackend.Library.DataTransferObjects.Product;

/// <summary>
/// Product Data Transfer Object
/// </summary>
public class ProductCreateDto
{
    /// <summary>
    /// Product Name
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required!")]
    public string? Name { get; set; }
    /// <summary>
    /// Product Description
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Description is required!")]
    public string? Description { get; set; }
    /// <summary>
    /// Product Cost
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Cost is required!")]
    public int Cost { get; set; }
    /// <summary>
    /// Discount
    /// </summary>
    public int? Discount { get; set; }
    //TODO
    /// <summary>
    /// Category Identifier
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Category is required!")]
    public int CategoryId { get; set; }
}