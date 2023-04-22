using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RPM_PR_LIB;

namespace RPM_Project_Backend.Domain;


public class CategoriesHaveAttribute
{
    [Column("cha_id")]
    public int Id { get; set; }
    [Column("cha_cat_id")]
    public int CategoryId { get; set; }
    [Column("cha_atr_id")]
    public int AttributeId { get; set; }

    [ValidateNever]
    public virtual Attribute ChaAtr { get; set; } = null!;

    [ValidateNever]
    public virtual Category ChaCat { get; set; } = null!;
}
