using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class RoleHasPermission
{
    [Column("rhp_id")]
    public int Id { get; set; }
    
    [Column("rhp_permission_id")]
    public int PermissionId { get; set; }
    [ValidateNever]
    public virtual Permission Permission { get; set; } = null!;
    
    [Column("rhp_role_id")]
    public int RoleId { get; set; }
    [ValidateNever]
    public virtual Role Role { get; set; } = null!;
}
