using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;

public class Order : BaseModel
{
    [Column("or_id")]
    public override int Id { get; set; }
    [Column("or_number")]
    public int Number { get; set; }
    [Column("or_status")]
    public string Status { get; set; }
    [Column("or_ad_id")]
    public int AddressId { get; set; }
    public Address Address { get; set; }
    [Column("or_u_id")]
    public int UserId { get; set; }
    public User User { get; set; }
    [Column("or_s_id")]
    public int SellerID { get; set; }
    public Seller Seller { get; set; }
    [Column("or_fcost")]
    public double Cost { get; set; }
    [Column("or_time")]
    public DateTime Time { get; set; }
}