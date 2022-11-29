using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;

/// <summary>
/// Программные продукты
/// </summary>
[Table("products")]
public class Product : BaseModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("pp_id")]
    public override long Id { get; set; }
    [Column("pp_publish_date")]
    public DateTime PublishDate { get; set; }
    [Column("pp_price")]
    public double Price { get; set; }
    [Column("pp_download_count")]
    public int DownloadCount { get; set; }
    [Column("pp_rating")] 
    public double Rating { get; set; }
    [Column("pp_publisher_id")]
    [ForeignKey("pp_publisher_id")]
    public long? PublisherId { get; set; }
    public Publisher? Publisher { get; set; }
}