using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_PR_LIB;


public partial class Review : BaseModel
{
    [Column("rew_id")]
    public override int Id { get; set; }

    [Column("rew_u_id")]
    public int UserId { get; set; }
    [ValidateNever]
    public virtual User User { get; set; } = null!;

    [Column("rew_text")]
    public string Text { get; set; } = null!;

    [Column("rew_grade")]
    public double Grade { get; set; }

    [Column("rew_pro_id")]
    public int ProductId { get; set; }
    [ValidateNever]
    [NotMapped]
    public virtual Product Product { get; set; } = null!;
}
