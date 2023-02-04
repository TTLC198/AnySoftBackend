using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;

public class Address : BaseModel
{
    [Column("ad_id")]
    public override int Id { get; set; }
    [Column("ad_street")]
    public string Street { get; set; }
    [Column("ad_city")]
    public string City { get; set; }
    [Column("ad_state")]
    public string State { get; set; }
}