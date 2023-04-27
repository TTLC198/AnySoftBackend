using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class Genre
{
    [Key]
    [Column("gen_id"), Required]
    public int Id { get; set; }
    [Column("gen_name"), Required, StringLength(50)]
    public string? Name { get; set; }

    [NotMapped]
    public virtual IEnumerable<Product>? Products { get; }
}
