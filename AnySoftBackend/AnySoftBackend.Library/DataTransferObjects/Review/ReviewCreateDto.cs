namespace AnySoftBackend.Library.DataTransferObjects.Review;

/// <summary>
/// Review Data Transfer Object
/// </summary>
public class ReviewCreateDto
{
    /// <summary>
    /// Review Text
    /// </summary>
    public string? Text { get; set; }
    /// <summary>
    /// Review Grade
    /// </summary>
    public double Grade { get; set; }
    /// <summary>
    /// Review Product Id
    /// </summary>
    public int ProductId { get; set; }
}