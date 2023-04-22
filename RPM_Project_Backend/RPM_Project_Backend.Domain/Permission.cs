using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using RPM_PR_LIB;

namespace RPM_Project_Backend.Domain;

[Index(nameof(Resource), IsUnique = true)]
public class Permission
{
    [Column("p_id")]
    public int Id { get; set; }
    [Column("p_resource")]
    public string Resource { get; set; } = null!;

    public virtual ICollection<RoleHasPermission> RoleHasPermissions { get; } = new List<RoleHasPermission>();
}
