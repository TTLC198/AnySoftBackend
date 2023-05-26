using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnySoftBackend.Domain;

public class Role
{
    [Key]
    [Column("r_id"), Required]   
    public int Id { get; set; }
    
    [Column("r_name"), Required, StringLength(15)]
    public string? Name { get; set; }
}
