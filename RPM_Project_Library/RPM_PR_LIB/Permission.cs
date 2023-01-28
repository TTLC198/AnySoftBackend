using System;
using System.Collections.Generic;

namespace RPM_PR_LIB;


public partial class Permission
{
    public int PId { get; set; }

    public string PResource { get; set; } = null!;

    public virtual ICollection<RoleHasPermission> RoleHasPermissions { get; } = new List<RoleHasPermission>();
}
