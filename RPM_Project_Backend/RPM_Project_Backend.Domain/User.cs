using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_Project_Backend.Domain;

/// <summary>
/// User Object
/// </summary>
public class User
{
    /// <summary>
    /// Identifier
    /// </summary>
    [Key]
    [Column("u_id"), Required]
    public int Id { get; set; }
    /// <summary>
    /// Login
    /// </summary>
    [Column("u_login"), Required]
    public string? Login { get; set; }
    /// <summary>
    /// Password
    /// </summary>
    [Column("u_password"), Required]
    public string? Password { get; set; }
    /// <summary>
    /// Email
    /// </summary>
    [Column("u_email"), Required]
    public string? Email { get; set; }
    /// <summary>
    /// Role identifier
    /// </summary>
    [Column("u_role_id"), Required]
    public int RoleId { get; set; }
    /// <summary>
    /// Role Object
    /// </summary>
    [ForeignKey("u_role_id")]
    public virtual Role? Role { get; set; }
    /// <summary>
    /// User addresses
    /// </summary>
    public virtual ICollection<Address> Addresses { get; } = new List<Address>();
}
/// <summary>
/// User Data Transfer Object
/// </summary>
public class UserDto
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
