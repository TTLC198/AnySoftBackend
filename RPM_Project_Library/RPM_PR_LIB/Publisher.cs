using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;

/// <summary>
/// Издатели
/// </summary>
[Table("publishers")]
public class Publisher : User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("p_id")]
    public long Id { get; set; }
    [Column("p_full_name")]
    public string? FullName { get; set; }
}