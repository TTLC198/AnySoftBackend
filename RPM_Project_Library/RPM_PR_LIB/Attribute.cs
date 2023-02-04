using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;

public class Attribute : BaseModel
{
    [Column("atr_id")]
    public override int Id { get; set; }
    [Column("atr_name")]
    public string Name { get; set; }
}