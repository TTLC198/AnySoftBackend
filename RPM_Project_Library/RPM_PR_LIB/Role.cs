using System;
using System.Collections.Generic;

namespace RPM_PR_LIB;


public partial class Role
{
    public int RId { get; set; }

    public string RName { get; set; } = null!;

    public virtual ICollection<RoleHasPermission> RoleHasPermissions { get; } = new List<RoleHasPermission>();

    public virtual ICollection<User> Users { get; } = new List<User>();
}
