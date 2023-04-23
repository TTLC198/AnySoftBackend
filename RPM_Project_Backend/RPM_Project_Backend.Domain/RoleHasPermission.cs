using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class RoleHasPermission
{
    [Key]
    [Column("rhp_id"), Required]
    public int Id { get; set; }
    
    [Column("rhp_permission_id"), Required]
    public int PermissionId { get; set; }

    [Column("rhp_role_id"), Required]
    public int RoleId { get; set; }

    [ValidateNever]
    public virtual Role? Role { get; set; }
    [ValidateNever]
    public virtual Permission Permission { get; set; }
}
