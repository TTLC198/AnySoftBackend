using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_PR_LIB;

/// <summary>
/// Пользователи
/// </summary>
[Table("users")]
public class User : BaseModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override long Id { get; set; }
    public string login { get; set; }
    public string password { get; set; }
    public string email { get; set; }
    public long RoleId { get; set; }
    public Role Role { get; set; }
}

