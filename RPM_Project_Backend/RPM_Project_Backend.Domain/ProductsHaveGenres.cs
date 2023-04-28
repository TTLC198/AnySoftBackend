using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class ProductsHaveGenres
{
    [Key]
    [Column("pha_id"), Required]
    public int Id { get; set; }

    [Column("pha_pro_id"), Required]
    public int ProductId { get; set; }

    [Column("pha_gen_id"), Required]
    public int GenreId { get; set; }

    [Column("pha_value"), Required, StringLength(50)]
    public string? Value { get; set; }

    [ValidateNever]
    [ForeignKey("pha_gen_id")]
    public virtual Genre? Genre { get; set; }
    [ValidateNever]
    [ForeignKey("pha_pro_id")]
    public virtual Product? Product { get; set; }
}
