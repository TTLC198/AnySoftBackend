using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_PR_LIB;

public class User : BaseModel
{
    [Column("u_id")]
    [ValidateNever]
    public override int Id { get; set; }
    [Column("u_login")]
    [ValidateNever]
    public string Login { get; set; } = null!;
    [Column("u_password")]
    [ValidateNever]
    public string Password { get; set; } = null!;
    [Column("u_email")]
    [ValidateNever]
    public string Email { get; set; } = null!;
    [Column("u_role_id")]
    [ValidateNever]
    public int RoleId { get; set; }

    [ValidateNever]
    public virtual Role Role { get; set; } = null!;
    public virtual ICollection<Address> Addresses { get; } = new List<Address>();
}
