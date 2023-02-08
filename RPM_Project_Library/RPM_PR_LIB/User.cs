using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;

public partial class User : BaseModel
{
    [Column("u_id")]
    public override int Id { get; set; }
    [Column("u_login")]
    public string Login { get; set; } = null!;
    [Column("u_balance")]
    public double Balance { get; set; }
    [Column("u_password")]
    public string Password { get; set; } = null!;
    [Column("u_email")]
    public string Email { get; set; } = null!;
    [Column("u_role_id")]
    public int RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;
}
