using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class ProductList
{
    [Key]
    [Column("pl_id"), Required]
    public int Id { get; set; }

    [Column("pl_u_id"), Required]
    public int UserId { get; set; }
    [Column("pl_name"), Required]
    public string? Name { get; set; }

    [ValidateNever]
    public virtual User? User { get; set; }
}
