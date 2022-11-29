using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;

/// <summary>
/// Комментарии
/// </summary>
[Table("comments")]
public class Comment
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("c_id")]
    public long Id { get; set; }
    [Column("c_publish_date")]
    public DateTime PublishDate { get; set; }
    [Column("c_product_id")]
    [ForeignKey("c_product_id")]
    public long? ProductId { get; set; }
    public Product? Product { get; set; }
    [Column("c_user_id")]
    [ForeignKey("c_user_id")]
    public long? UserId { get; set; }
    public User? User { get; set; }
}