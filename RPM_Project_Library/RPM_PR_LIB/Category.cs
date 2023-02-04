using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;

public partial class Category : BaseModel
{
    [Column("cat_id")]
    public override int Id { get; set; }
    [Column("cat_parent_id")]
    public int ParentCategoryId { get; set; }
    public Category ParentCategory { get; set; }
    [Column("cat_name")]
    public string Name { get; set; }
}