using System.ComponentModel.DataAnnotations.Schema;

namespace RPM_Project_Backend.Domain;

public class Role
{
    [Column("r_id")]   
    public int Id { get; set; }
    
    [Column("r_name")]
    public string Name { get; set; } = null!;
}
