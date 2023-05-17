using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class Review
{
    [Key]
    [Column("rew_id"), Required]
    public int Id { get; set; }

    [Column("rew_u_id"), Required]
    public int UserId { get; set; }

    [Column("rew_text"), Required, StringLength(256)]
    public string? Text { get; set; }

    [Column("rew_grade"), Required]
    public double Grade { get; set; }   

    [Column("rew_pro_id"), Required]
    public int ProductId { get; set; }
    
    [Column("rew_ts"), Required]
    public DateTime Ts { get; set; }

    [ValidateNever]
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; }

    [ValidateNever]
    [ForeignKey("UserId")]
    [NotMapped]
    public virtual User? User { get; }
}
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

/// <summary>
/// Review Data Transfer Object
/// </summary>
public class ReviewDto
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
