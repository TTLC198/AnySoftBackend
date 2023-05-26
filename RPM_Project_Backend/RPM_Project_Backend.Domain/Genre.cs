using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace RPM_Project_Backend.Domain;

[Index(nameof(Id), IsUnique = true)]
public class Genre
{
    [Key]
    [Column("gen_id"), Required]
    public int Id { get; set; }
    [Column("gen_name"), Required, StringLength(50)]
    public string? Name { get; set; }
    
    [ValidateNever]
    [JsonIgnore]
    public virtual IEnumerable<ProductsHaveGenres>? ProductsHaveGenres { get; }
}