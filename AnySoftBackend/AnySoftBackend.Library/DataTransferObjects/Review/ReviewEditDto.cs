namespace AnySoftBackend.Library.DataTransferObjects.Review;

/// <summary>
/// Review object to edit
/// </summary>
public class ReviewEditDto
{
    /// <summary>
    /// Identifier
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Review Text
    /// </summary>
    public string? Text { get; set; }
    /// <summary>
    /// Review Grade
    /// </summary>
    public double Grade { get; set; }
}