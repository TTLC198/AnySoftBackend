using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_PR_LIB;

public class User : BaseModel
{
    [Column("u_id")]
    public override int Id { get; set; }
    [Column("u_login")]
    public string Login { get; set; } = null!;
    [Column("u_password")]
    public string Password { get; set; } = null!;
    [Column("u_email")]
    public string Email { get; set; } = null!;
    [Column("u_role_id")]
    public int RoleId { get; set; }
    
    public virtual Role Role { get; set; } = null!;
    public virtual ICollection<Address> Addresses { get; } = new List<Address>();
}

public class UserDto
{
    public string? Login { get; set; }

    public string? Email { get; set; }

    [Required(ErrorMessage = "User Password is required!")]
    public string? Password { get; set; }

    [Range(typeof(bool), "true", "true", ErrorMessage = "At least one field must be given a value")]
    public bool IsValidated =>
        !string.IsNullOrWhiteSpace(Login) || !string.IsNullOrWhiteSpace(Email);
}
