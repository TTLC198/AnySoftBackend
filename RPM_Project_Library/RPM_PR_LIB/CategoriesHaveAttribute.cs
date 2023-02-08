using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;


public partial class CategoriesHaveAttribute
{
    [Column("cha_id")]
    public int Id { get; set; }
    [Column("cha_cat_id")]
    public int CategoryId { get; set; }
    [Column("cha_atr_id")]
    public int AttributeId { get; set; }

    public virtual Attribute ChaAtr { get; set; } = null!;

    public virtual Category ChaCat { get; set; } = null!;
}
