namespace AnySoftBackend.Library.DataTransferObjects.User;

/// <summary>
/// User Data Transfer Object
/// </summary>
public class UserEditDto
{
    /// <summary>
    /// Identifier
    /// </summary>
    public int? Id { get; set; }
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
    public string? Password { get; set; }
}