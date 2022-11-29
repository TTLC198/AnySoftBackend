using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;

/// <summary>
/// Заказ программного продукта
/// </summary>
[Table("product_order")]
public class ProductOrder
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("po_id")]
    public long Id { get; set; }
    [Column("po_product_id")]
    [ForeignKey("po_product_id")]
    public long? ProductId { get; set; }
    public Product? Product { get; set; }
    [Column("po_user_id")]
    [ForeignKey("po_user_id")]
    public long? UserId { get; set; }
    public User? User { get; set; }
}
