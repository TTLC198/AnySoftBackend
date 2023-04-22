using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RPM_PR_LIB;

namespace RPM_Project_Backend.Domain;

public class ProductList
{
    [Column("pl_id")]
    public int Id { get; set; }

    [Column("pl_u_id")]
    public int UserId { get; set; }
    [ValidateNever]
    public virtual User User { get; set; } = null!;

    [Column("pl_name")]
    public string Name { get; set; } = null!;
}
