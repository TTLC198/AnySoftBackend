using System;
using AnySoftBackend.Library.DataTransferObjects.User;

namespace AnySoftBackend.Library.DataTransferObjects.Review;

/// <summary>
/// Review object that is returned when requested
/// </summary>
public class ReviewResponseDto
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
    /// <summary>
    /// Review creation date
    /// </summary>
    public DateTime Ts { get; set; }
    /// <summary>
    /// Review Product Id
    /// </summary>
    public int ProductId { get; set; }
    /// <summary>
    /// Review check is own
    /// </summary>
    public bool IsOwn { get; set; }
    /// <summary>
    /// Review User entity
    /// </summary>
    public UserResponseDto? User { get; set; }
}