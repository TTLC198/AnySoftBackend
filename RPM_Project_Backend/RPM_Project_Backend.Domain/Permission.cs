using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RPM_Project_Backend.Domain;

[Index(nameof(Resource), IsUnique = true)]
public class Permission
{
    [Key]
    [Column("p_id"), Required]
    public int Id { get; set; }
    [Column("p_resource"), Required]
    public string? Resource { get; set; }

    public virtual IEnumerable<RoleHasPermission> RoleHasPermissions { get; } = new List<RoleHasPermission>();
}
