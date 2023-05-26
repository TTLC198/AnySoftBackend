using System.ComponentModel.DataAnnotations;

namespace AnySoftBackend.Library.DataTransferObjects.User;

/// <summary>
/// User Data Transfer Object
/// </summary>
public class UserCreateDto
{
    /// <summary>
    /// Login
    /// </summary>
    public string? Login { get; set; }
    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }
    /// <summary>
    /// Password
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "User Password is required!")]
    public string? Password { get; set; }
    /// <summary>
    /// Equals true if Email or Login exists
    /// </summary>
    [Range(typeof(bool), "true", "true", ErrorMessage = "At least one field must be given a value")]
    public bool IsValidated =>
        !string.IsNullOrWhiteSpace(Login) || !string.IsNullOrWhiteSpace(Email);
}