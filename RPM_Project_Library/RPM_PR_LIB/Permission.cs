using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RPM_PR_LIB;

[Index(nameof(Resource), IsUnique = true)]
public partial class Permission : BaseModel
{
    [Column("p_id")]
    public override int Id { get; set; }
    [Column("p_resource")]
    public string Resource { get; set; } = null!;

    public virtual ICollection<RoleHasPermission> RoleHasPermissions { get; } = new List<RoleHasPermission>();
}
