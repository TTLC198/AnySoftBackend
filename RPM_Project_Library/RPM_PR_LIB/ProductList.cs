using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_PR_LIB;


public partial class ProductList : BaseModel
{
    [Column("pl_id")]
    public override int Id { get; set; }

    [Column("pl_u_id")]
    public int UserId { get; set; }
    [ValidateNever]
    public virtual User User { get; set; } = null!;

    [Column("pl_name")]
    public string Name { get; set; } = null!;
}
