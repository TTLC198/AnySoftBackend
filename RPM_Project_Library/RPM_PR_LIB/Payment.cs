using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;

public class Payment : BaseModel
{
    [Column("pay_id")]
    public override int Id { get; set; }
    [Column("pay_u_id")]
    public int UserId { get; set; }
    [Column("pay_method")]
    public string Method { get; set; }
}