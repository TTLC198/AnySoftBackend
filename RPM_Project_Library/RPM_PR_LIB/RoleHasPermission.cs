using System;
using System.Collections.Generic;

namespace RPM_PR_LIB;


public partial class RoleHasPermission
{
    public int RhpRoleId { get; set; }

    public int RhpPermissionId { get; set; }

    public int RhpId { get; set; }

    public virtual Permission RhpPermission { get; set; } = null!;

    public virtual Role RhpRole { get; set; } = null!;
}
