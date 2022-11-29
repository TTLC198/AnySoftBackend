using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;

/// <summary>
/// Модераторы
/// </summary>
[Table("moderators")]
public class Moderator : User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("m_id")]
    public long Id { get; set; }
    [Column("m_full_name")]
    public string? FullName { get; set; }
}