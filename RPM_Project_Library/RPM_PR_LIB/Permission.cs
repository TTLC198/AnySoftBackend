using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;


public partial class Permission : BaseModel
{
    [Column("p_id")]
    public override int Id { get; set; }
    [Column("p_resource")]
    public string Resource { get; set; } = null!;

    public virtual ICollection<RoleHasPermission> RoleHasPermissions { get; } = new List<RoleHasPermission>();
}
