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
    [Column("u_id")]
    public override long Id { get; set; }
    [Required (ErrorMessage = "Необходим никнейм пользователя")]
    [Column("u_nickname")]
    public string Nickname { get; set; }
    [Required (ErrorMessage = "Необходим пароль пользователя")]
    [Column("u_password")]
    public string Password { get; set; }
    [Column("u_balance")]
    public double Balance { get; set; }
}