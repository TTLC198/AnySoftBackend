using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;


public class CategoriesHaveAttribute
{
    [Key]
    [Column("cha_id"), Required]
    public int Id { get; set; }
    [Column("cha_cat_id"), Required]
    public int CategoryId { get; set; }
    [Column("cha_atr_id"), Required]
    public int AttributeId { get; set; }

    [ValidateNever]
    public virtual Attribute ChaAtr { get; set; } = null!;

    [ValidateNever]
    public virtual Category ChaCat { get; set; } = null!;
}
